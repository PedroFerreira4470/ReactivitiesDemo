import React, { useContext } from 'react'
import { Card, Image, Button, ButtonGroup } from 'semantic-ui-react'
import ActivityStore from "../../../app/stores/activityStore";
import { observer } from 'mobx-react-lite';



const ActivityDetails: React.FC = () => {
  const activityStore = useContext(ActivityStore);
  const {selectedActivity: activity,openEditForm,cancelSelectedActivity} = activityStore;
    return (
        <Card fluid>
        <Image src={`/assets/categoryImages/${activity!.category}.jpg`} wrapped ui={false} />
        <Card.Content>
          <Card.Header>{activity!.title}</Card.Header>
          <Card.Meta>
            <span>{activity!.date}</span>
          </Card.Meta>
          <Card.Description>
            {activity!.description}
          </Card.Description>
        </Card.Content>
        <Card.Content extra>
          <ButtonGroup widths={2}>
            <Button onClick={() => openEditForm(activity!.id)} basic color="blue" content="edit"/>
            <Button onClick={cancelSelectedActivity} basic color="grey" content="cancel"/>
          </ButtonGroup>
        </Card.Content>
      </Card>
    )
}

export default observer(ActivityDetails);
