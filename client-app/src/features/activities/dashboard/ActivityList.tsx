import React, { useContext } from "react";
import { Item, Button, Label, Segment } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import ActivityStore from "../../../app/stores/activityStore";



const ActivityList: React.FC = () => {
  const activityStore = useContext(ActivityStore);
  const {activitiesByDate,selectActivity,deleteActivity,target,submiting} = activityStore;

  return (
    <Segment clearing>
      <Item.Group divided>
        {activitiesByDate.map(act => (
          <Item key={act.id}>
            <Item.Content>
              <Item.Header as="a">{act.title}</Item.Header>
              <Item.Meta>{act.date}</Item.Meta>
              <Item.Description>
                <p>{act.description}</p>
                <p>
                  {act.city} , {act.venue}
                </p>
              </Item.Description>
              <Item.Extra>
                <Button
                  onClick={() => {selectActivity(act.id);}}
                  floated="right"
                  content="View"
                  color="blue"
                />
                <Button
                  name={act.id}
                  onClick={(e) => {deleteActivity(e,act.id);}}
                  loading={ target===act.id && submiting}
                  floated="right"
                  content="Delete"
                  color="red"
                />
                <Label basic content={act.category} />
              </Item.Extra>
            </Item.Content>
          </Item>
        ))}
      </Item.Group>
    </Segment>
  );
};
export default observer(ActivityList);
