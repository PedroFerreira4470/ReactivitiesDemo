import { observable, action, computed, configure, runInAction } from "mobx";
import { createContext, SyntheticEvent } from "react";
import { IActivity } from "../models/activity";
import agent from "../api/agent";
import { history } from "../..";
import { toast } from "react-toastify";

configure({ enforceActions: "always" });

class ActivityStore {
  @observable activityRegistry = new Map();
  @observable activity: IActivity | null = null;
  @observable loadingInitial = false;
  @observable submiting = false;
  @observable target = "";

  @computed get activitiesByDate() {
    return this.groupActivitiesByDate(Array.from(this.activityRegistry.values()));
  }

  groupActivitiesByDate(activities: IActivity[]){
    const sortedActivities = activities.sort(
      (a, b) => b.date!.getTime() - a.date!.getTime()
      );
    
    return Object.entries(sortedActivities.reduce((activities, activity) => {
      const date = activity.date.toISOString().split('T')[0];
      activities[date] = activities[date] ? [...activities[date],activity] : [activity];
      return activities;
    }, {} as {[key: string] : IActivity[]}));

  }

  @action loadActivities = async () => {
    this.loadingInitial = true;
    try {
      const activities = await agent.Activities.list();
      runInAction("loading activities", () => {
        activities.forEach(act => {
          act.date = new Date(act.date!);
          this.activityRegistry.set(act.id, act);
        });
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
      return activity;
    } else {
      this.loadingInitial = true;
      try {
        const activity = await agent.Activities.detail(id);
        runInAction("get activity", () => {
          activity.date = new Date(activity.date);
          this.activity = activity;
          this.activityRegistry.set(activity.id, activity);
        });
        return activity;
      } catch (error) {
        console.log(error);
      }finally{
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
      runInAction("create activity", () => {
        this.activityRegistry.set(activity.id, activity);
      });
      history.push(`/activity/${activity.id}`)
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
        history.push(`/activity/${activity.id}`)
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


}

export default createContext(new ActivityStore());
