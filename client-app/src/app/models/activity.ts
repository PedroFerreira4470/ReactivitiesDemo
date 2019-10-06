import { runInAction } from "mobx";

export interface IActivitiesEnvelope{
    activities: IActivity[];
    activityCount:number;
}
export interface IActivity {
    id: string,
    title:string,
    category: string,
    description:string,
    date: Date,
    city:string,
    venue:string,
    isGoing:boolean,
    isHost:boolean,
    attendees: IAttendee[],
    comments: IComment[],
}

export interface IComment {
    id: string,
    createAt: Date,
    body: string,
    userName: string,
    displayName: string,
    image: string,
}

export interface IActivityForm extends Partial<IActivity> {
    time?:Date
}

export class ActivityFormValues implements IActivityForm{
    id?: string = undefined;
    title: string = "";
    category: string = "";
    description: string = "";
    date?: Date = undefined;
    time?: Date = undefined;
    city: string = "";
    venue: string = "";
    constructor(init?: IActivityForm){
        if(init && init.date){
            runInAction(() =>{
                init.time = init.date;
            });
        }
        Object.assign(this,init);
    }
}

export interface IAttendee {
    userName: string,
    displayName: string,
    image: string,
    isHost: boolean,
    following?: boolean,
}
