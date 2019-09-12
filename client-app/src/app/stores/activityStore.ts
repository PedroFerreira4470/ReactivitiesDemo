import { observable, action, computed, configure, runInAction } from "mobx";
import { createContext, SyntheticEvent } from "react";
import { IActivity } from "../models/activity";
import agent from "../api/agent";

configure({ enforceActions: "always" });

class ActivityStore {
  @observable activityRegistry = new Map();
  @observable activities: IActivity[] = [];
  @observable selectedActivity: IActivity | undefined = undefined;
  @observable loadingInitial = false;
  @observable editMode = false;
  @observable submiting = false;
  @observable target = "";

  @computed get activitiesByDate() {
    return Array.from(this.activityRegistry.values()).sort(
      (a, b) => Date.parse(b.date) - Date.parse(a.date)
    );
  }

  @action loadActivities = async () => {
    this.loadingInitial = true;
    try {
      const activities = await agent.Activities.list();
      runInAction('loading activities',() => {
        activities.forEach(act => {
          act.date = act.date.split(".")[0];
          this.activityRegistry.set(act.id, act);
        });
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction('finish loading activities',() => {
        this.loadingInitial = false;
      });
    }
  };

  @action createActivity = async (activity: IActivity) => {
    this.submiting = false;
    try {
      await agent.Activities.create(activity);
      runInAction('create activity',() => {
        this.activityRegistry.set(activity.id, activity);
        this.editMode = false;
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction('finish creating activity',() => {
        this.submiting = false;
      });
    }
  };

  @action editActivity = async (activity: IActivity) => {
    this.submiting = true;
    try {
      await agent.Activities.update(activity);
      runInAction('editing activity',() => {
        this.activityRegistry.set(activity.id, activity);
        this.editMode = false;
        this.selectedActivity = activity;
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction('finish editing activity',() => {
        this.submiting = false;
      });
    }
  };

  @action deleteActivity = async (
    event: SyntheticEvent<HTMLButtonElement>,
    id: string
  ) => {
    this.editMode = false;
    this.submiting = true;
    this.target = event.currentTarget.name;
    try {
      await agent.Activities.delete(id);
      runInAction('delete activity',() => {
        this.activityRegistry.delete(id);
      });
    } catch (error) {
      console.error(error);
    } finally {
      runInAction('finish deleting activity',() => {
        this.submiting = false;
        this.target = "";
      });
    }
  };

  @action openCreateForm = () => {
    this.editMode = true;
    this.selectedActivity = undefined;
  };

  @action openEditForm = (id: string) => {
    this.selectedActivity = this.activityRegistry.get(id);
    this.editMode = true;
  };
  @action cancelSelectedActivity = () => {
    this.selectedActivity = undefined;
  };

  @action cancelFormOpen = () => {
    this.editMode = false;
  };

  @action selectActivity = (id: string) => {
    this.selectedActivity = this.activityRegistry.get(id);
    this.editMode = false;
  };
}

export default createContext(new ActivityStore());
