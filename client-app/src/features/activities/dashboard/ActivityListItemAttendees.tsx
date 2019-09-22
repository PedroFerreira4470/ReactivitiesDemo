import React from "react";
import { List, Image, Popup } from "semantic-ui-react";
import { IAttendee } from "../../../app/models/activity";

interface IProps {
  attendees: IAttendee[];
}

const ActivityListItemAttendees: React.FC<IProps> = ({ attendees }) => {
  if (attendees && attendees.length > 0) {
    return (
      <List horizontal>
        {attendees.map(attend => (
          <List.Item key={attend.userName} >
            <Popup
              header={attend.displayName}
              trigger={
                <Image
                  size="mini"
                  circular
                  src={attend.image || "/assets/user.png"}
                />
              }
            />
          </List.Item>
        ))}
      </List>
    );
  } else {
    return <p>No attendees</p>;
  }
};

export default ActivityListItemAttendees;
