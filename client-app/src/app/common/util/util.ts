import { IActivity, IAttendee } from "../../models/activity";
import { IUser } from "../../models/user";

export  const combineDateAndTime = (date:Date, time:Date)=>{
   
    const dateString = date.toISOString().split('T')[0];
    const timeString = time.toISOString().split('T')[1];

    return new Date(dateString +'T'+ timeString);
}

export const setActivityProps = (activity: IActivity, user: IUser) => {
    activity.date = new Date(activity.date);

    activity.isGoing = activity.attendees.some(
      a => a.userName === user.userName
    )
    activity.isHost = activity.attendees.some(
      a => a.userName === user.userName && a.isHost
    )
    return activity;
}


export const createAttendee = (user: IUser): IAttendee => {
    return {
        displayName: user.displayName,
        isHost: false,
        userName:user.userName,
        image:user.image!
    }
}

export const createAttendeeHasHost = (user: IUser): IAttendee => {
  return {
      displayName: user.displayName,
      isHost: true,
      userName:user.userName,
      image:user.image!
  }
}