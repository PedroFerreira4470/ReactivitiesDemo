import React, { useContext } from "react";
import { Item, Button, Segment, Icon, Label } from "semantic-ui-react";
import { IActivity } from "../../../app/models/activity";
import { Link } from "react-router-dom";
import { observer } from "mobx-react-lite";
import { format } from "date-fns";
import { RootStoreContext } from "../../../app/stores/rootStore";
import ActivityListItemAttendees from "./ActivityListItemAttendees";

interface IProps {
  activity: IActivity;
}
const ActivityListItem: React.FC<IProps> = ({ activity }) => {
  const host = activity.attendees.filter(x => x.isHost)[0];
  const rootStore = useContext(RootStoreContext);
  const { deleteActivity, target, submiting } = rootStore.activityStore;
  //console.log("going: "+activity.isGoing + " host:"+activity.isHost);
  return (
    <Segment.Group>
      <Segment>
        <Item.Group>
          <Item>
            <Item.Image
              size="tiny"
              circular
              src={host.image || "/assets/user.png"}
              style={{ marginBottom: 10 }}
            />
            <Item.Content>
              <Item.Header as={Link} to={`/activity/${activity.id}`}>
                {activity.title}
              </Item.Header>
              <Item.Description>
                Hosted By 
                <Link to={`profile/${host.userName}`}> {host.displayName}</Link>
              </Item.Description>
              {activity.isHost && (
                <Item.Description>
                  <Label
                    color="orange"
                    basic
                    content="You are hosting this activity"
                  />
                </Item.Description>
              )}

              {activity.isGoing && !activity.isHost && (
                <Item.Description>
                  <Label
                    color="green"
                    basic
                    content="You are going to this activity"
                  />
                </Item.Description>
              )}
            </Item.Content>
          </Item>
        </Item.Group>
      </Segment>
      <Segment>
        <Icon name="clock" /> {format(activity.date, "h:mm a")}
        <Icon name="marker" /> {activity.venue},{activity.city}
      </Segment>
      <Segment secondary>
        <ActivityListItemAttendees attendees={activity.attendees} />
      </Segment>
      <Segment clearing>
        <span>{activity.description}</span>
        <Button
          as={Link}
          to={`/activity/${activity.id}`}
          floated="right"
          content="View"
          color="blue"
        />
        <Button
          name={activity.id}
          onClick={e => {
            deleteActivity(e, activity.id);
          }}
          loading={target === activity.id && submiting}
          floated="right"
          content="Delete"
          color="red"
        />
      </Segment>
    </Segment.Group>
  );
};

export default observer(ActivityListItem);
