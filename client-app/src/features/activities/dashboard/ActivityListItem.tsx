import React, { useContext } from "react";
import { Item, Button, Segment, Icon } from "semantic-ui-react";
import { IActivity } from "../../../app/models/activity";
import { Link } from "react-router-dom";
import ActivityStore from "../../../app/stores/activityStore";

interface IProps {
  activity: IActivity;
}
const ActivityListItem: React.FC<IProps> = ({ activity }) => {
  const activityStore = useContext(ActivityStore);
  const { deleteActivity, target, submiting } = activityStore;

  return (
    <Segment.Group>
      <Segment>
        <Item.Group>
          <Item>
            <Item.Image size="tiny" circular src="/assets/user.png" />
            <Item.Content>
              <Item.Header as="a">{activity.title}</Item.Header>
              <Item.Description>Hosted By Bob</Item.Description>
            </Item.Content>
          </Item>
        </Item.Group>
      </Segment>
      <Segment>
        <Icon name="clock" /> {activity.date}
        <Icon name="marker" /> {activity.venue},{activity.city}
      </Segment>
      <Segment secondary>attends will go here</Segment>
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

export default ActivityListItem;
