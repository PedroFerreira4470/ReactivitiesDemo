import React, { Fragment } from "react";
import { Segment, List, Item, Label, Image } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { IAttendee } from "../../../app/models/activity";
import { observer } from "mobx-react-lite";

interface IProps {
  attendees: IAttendee[];
}
const ActivityDetailedSideBar: React.FC<IProps> = ({ attendees }) => {
  return (
    <Fragment>
      <Segment
        textAlign="center"
        style={{ border: "none" }}
        attached="top"
        secondary
        inverted
        color="teal"
      >
        {attendees.length} {attendees.length === 1 ? "person" : "people"} going
      </Segment>
      <Segment attached>
        <List relaxed divided>
          {attendees.slice()
            .sort(attend => (attend.isHost ? -1 : 1))
            .map(attend => (
              <Item key={attend.userName} style={{ position: "relative" }}>
                {attend.isHost && (
                  <Label
                    style={{ position: "absolute" }}
                    color="orange"
                    ribbon="right"
                  >
                    Host
                  </Label>
                )}
                <Image size="tiny" src={attend.image || "/assets/user.png"} />
                <Item.Content verticalAlign="middle">
                  <Item.Header as="h3">
                    <Link to={`/profile/${attend.userName}`}>
                      {attend.displayName}
                    </Link>
                  </Item.Header>
                  <Item.Extra style={{ color: "orange" }}>Following</Item.Extra>
                </Item.Content>
              </Item>
            ))}
        </List>
      </Segment>
    </Fragment>
  );
};

export default observer(ActivityDetailedSideBar);
