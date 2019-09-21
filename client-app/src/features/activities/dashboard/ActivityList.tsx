import React, { useContext, Fragment } from "react";
import { Item, Label } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import ActivityListItem from "./ActivityListItem";
import { RootStoreContext } from "../../../app/stores/rootStore";
import {format} from "date-fns";
 
const ActivityList: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const {activitiesByDate} = rootStore.activityStore;

  return (
    <Fragment>
      {activitiesByDate.map(([date, activities]) => (
        <Fragment key={date}>
          <Label size="large" color="blue" content={format(date,"eeee do MMMM")} />
            <Item.Group divided>
              {activities.map(act => (
                <ActivityListItem key={act.id} activity={act} />
              ))}
            </Item.Group>
        </Fragment>
      ))}
    </Fragment>
  );
};
export default observer(ActivityList); 
