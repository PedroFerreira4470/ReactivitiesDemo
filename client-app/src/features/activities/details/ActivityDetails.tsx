import React, { useContext, useEffect } from 'react'
import { Card, Image, Button, ButtonGroup } from 'semantic-ui-react'
import ActivityStore from "../../../app/stores/activityStore";
import { observer } from 'mobx-react-lite';
import { RouteComponentProps } from 'react-router';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { Link } from 'react-router-dom';

interface DetailParams {
  id:string
}

const ActivityDetails: React.FC<RouteComponentProps<DetailParams>> = ({match,history}) => {


  const activityStore = useContext(ActivityStore);
  const {activity,loadActivity,loadingInitial} = activityStore;

  useEffect(()=>{
    loadActivity(match.params.id);
  }, [loadActivity,match.params.id]);

  if (loadingInitial || !activity) return <LoadingComponent/> 

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
            <Button as ={Link} to={`/manage/${activity!.id}`} basic color="blue" content="edit"/>
            <Button onClick={() => history.push('/activities')} basic color="grey" content="cancel"/>
          </ButtonGroup>
        </Card.Content>
      </Card>
    )
}

export default observer(ActivityDetails);
