import { observable, action, computed, runInAction, reaction, toJS } from "mobx";
import { SyntheticEvent } from "react";
import { IActivity } from "../models/activity";
import agent from "../api/agent";
import { history } from "../..";
import { toast } from "react-toastify";
import { RootStore } from "./rootStore";
import {
  setActivityProps,
  createAttendee,
  createAttendeeHasHost
} from "../common/util/util";
import { HubConnection, HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

const LIMIT = 2;

export default class ActivityStore {
  rootStore: RootStore;
  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;

    reaction(
      () => this.predicate.keys(),
      () => {
        this.page = 0;
        this.activityRegistry.clear();
        this.loadActivities();
      }
    )
  }

  @observable activityRegistry = new Map();
  @observable activity: IActivity | null = null;
  @observable loadingInitial = false;
  @observable submiting = false;
  @observable target = "";
  @observable loading = false;
  @observable.ref HubConnection: HubConnection | null = null;
  @observable activityCount = 0;
  @observable page = 0;
  @observable predicate = new Map();

  @action setPredicate = (predicate: string, value: string | Date) => {
    this.predicate.clear();
    if (predicate !== "all") {
      this.predicate.set(predicate, value);
    }
  };

  @computed get axiosParams() {
    const params = new URLSearchParams();
    params.append("limit", String(LIMIT));
    params.append("offset", `${this.page ? (this.page * LIMIT) : 0}`);
    this.predicate.forEach((value, key) => {
      if (key === "startDate") {
        params.append(key, value.toISOString());
      } else {
        params.append(key, value);
      }
    });
    return params;
  }

  @computed get TotalPages() {
    return Math.ceil(this.activityCount / LIMIT);
  }

  @action setPage = (page: number) => {
    this.page = page;
  };

  @action createHubConnection = () => {
    this.HubConnection = new HubConnectionBuilder()
      .withUrl(process.env.REACT_APP_API_CHAT_URL!, {
        accessTokenFactory: () => this.rootStore.commonStore.token!
      })
      .configureLogging(LogLevel.Information)
      .build();

    this.HubConnection.start()
      .then(() => console.log(this.HubConnection!.state))
      .catch(error => console.log("Error establising connection:", error));

    this.HubConnection.on("Receive Comment", comment => {
      runInAction(() => {
        this.activity!.comments.push(comment);
      });
    });
  };

  @action stopHubConnection = () => {
    this.HubConnection!.stop();
  };

  @action addComment = async (values: any) => {
    values.ActivityId = this.activity!.id;
    try {
      await this.HubConnection!.invoke("SendComment", values);
    } catch (error) {
      console.log(error);
    }
  };

  @computed get activitiesByDate() {
    return this.groupActivitiesByDate(
      Array.from(this.activityRegistry.values())
    );
  }

  groupActivitiesByDate(activities: IActivity[]) {
    const sortedActivities = activities.sort(
      (a, b) => a.date!.getTime() - b.date!.getTime()
    );

    return Object.entries(
      sortedActivities.reduce(
        (activities, activity) => {
          const date = activity.date.toISOString().split("T")[0];
          activities[date] = activities[date]
            ? [...activities[date], activity]
            : [activity];
          return activities;
        },
        {} as { [key: string]: IActivity[] }
      )
    );
  }

  @action loadActivities = async () => {
    this.loadingInitial = true;
    try {
      const activitiesEnvelope = await agent.Activities.list(this.axiosParams);
      const { activities, activityCount } = activitiesEnvelope;
      runInAction("loading activities", () => {
        activities.forEach(act => {
          setActivityProps(act, this.rootStore.userStore.user!);
          this.activityRegistry.set(act.id, act);
        });
        this.activityCount = activityCount;
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction("finish loading activities", () => {
        this.loadingInitial = false;
      });
    }
  };

  @action loadActivity = async (id: string) => {
    let activity = this.getActivity(id);
    if (activity) {
      this.activity = activity;
      return toJS(activity);
    } else {
      this.loadingInitial = true;
      try {
        const activity = await agent.Activities.detail(id);
        runInAction("get activity", () => {
          setActivityProps(activity, this.rootStore.userStore.user!);
          this.activity = activity;
          this.activityRegistry.set(activity.id, activity);
        });
        return activity;
      } catch (error) {
        console.log(error);
      } finally {
        runInAction("finished getting activity", () => {
          this.loadingInitial = false;
        });
      }
    }
  };

  @action clearActivity = () => {
    this.activity = null;
  };

  getActivity(id: string) {
    return this.activityRegistry.get(id);
  }

  @action createActivity = async (activity: IActivity) => {
    this.submiting = true;
    try {
      await agent.Activities.create(activity);
      const attendee = createAttendeeHasHost(this.rootStore.userStore.user!);
      activity.attendees = [attendee];
      activity.comments = [];
      activity.isHost = true;
      runInAction("create activity", () => {
        this.activityRegistry.set(activity.id, activity);
      });
      history.push(`/activity/${activity.id}`);
    } catch (error) {
      toast.error("error sending data");
      console.log(error.response);
    } finally {
      runInAction("finish creating activity", () => {
        this.submiting = false;
      });
    }
  };

  @action editActivity = async (activity: IActivity) => {
    this.submiting = true;
    try {
      await agent.Activities.update(activity);
      runInAction("editing activity", () => {
        this.activityRegistry.set(activity.id, activity);
        this.activity = activity;
        history.push(`/activity/${activity.id}`);
      });
    } catch (error) {
      toast.error("error sending data");
      console.log(error);
    } finally {
      runInAction("finish editing activity", () => {
        this.submiting = false;
      });
    }
  };

  @action deleteActivity = async (
    event: SyntheticEvent<HTMLButtonElement>,
    id: string
  ) => {
    this.submiting = true;
    this.target = event.currentTarget.name;
    try {
      await agent.Activities.delete(id);
      runInAction("delete activity", () => {
        this.activityRegistry.delete(id);
      });
    } catch (error) {
      console.error(error);
    } finally {
      runInAction("finish deleting activity", () => {
        this.submiting = false;
        this.target = "";
      });
    }
  };

  @action attendActivity = async () => {
    const attendee = createAttendee(this.rootStore.userStore.user!);
    this.loading = true;
    try {
      await agent.Activities.attend(this.activity!.id);
      runInAction(() => {
        if (this.activity) {
          this.activity.attendees.push(attendee);
          this.activity.isGoing = true;
          this.activityRegistry.set(this.activity.id, this.activity);
        }
      });
    } catch (error) {
      toast.error("Problem signing up to activity");
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  @action cancelAttendance = async () => {
    const user = this.rootStore.userStore.user!;
    this.loading = true;
    try {
      await agent.Activities.unattend(this.activity!.id);
      runInAction(() => {
        if (this.activity) {
          this.activity.attendees = this.activity.attendees.filter(
            a => a.userName !== user.userName
          );
          this.activity.isGoing = false;
          this.activityRegistry.set(this.activity.id, this.activity);
        }
      });
    } catch (error) {
      toast.error("Problem cancelling up attendence");
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };
}
