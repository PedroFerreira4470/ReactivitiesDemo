import React, { useState, useContext, useEffect } from "react";
import { Segment, Form, Button, Grid } from "semantic-ui-react";
import { IActivityForm, ActivityFormValues } from "../../../app/models/activity";
import ActivityStore from "../../../app/stores/activityStore";
import { observer } from "mobx-react-lite";
import { RouteComponentProps } from "react-router";
import { Form as FinalForm, Field } from "react-final-form";
import TextInput from "../../../app/common/form/TextInput";
import TextAreaInput from "../../../app/common/form/TextAreaInput";
import SelectInput from "../../../app/common/form/SelectInput";
import { category } from "../../../app/common/options/CategoryOptions";
import DateInput from "../../../app/common/form/DateInput";
import { combineDateAndTime } from "../../../app/common/util/util";
import {combineValidators, isRequired,composeValidators,hasLengthGreaterThan} from "revalidate";
import uuid from "uuid";


const validate = combineValidators({
  title: isRequired({message:'The event Title is required'}),
  category: isRequired({message:'The Category is required'}),
  description: composeValidators(
    isRequired({message:'The Description is required'}),
    hasLengthGreaterThan(4)({message:'The Description needs at least 4 characters'})
  )(),
  city: isRequired({message:'The city is required'}),
  venue: isRequired({message:'The venue is required'}),
  date: isRequired({message:'The date is required'}),
  time: isRequired({message:'The time is required'}),
})

interface DetailParams {
  id: string;
}

const ActivityForm: React.FC<RouteComponentProps<DetailParams>> = ({
  match,
  history
}) => {
  const activityStore = useContext(ActivityStore);
  const { id: activityId } = match.params;
  const { submiting, loadActivity,createActivity,editActivity } = activityStore;
  const [activity, setActivity] = useState<IActivityForm>(new ActivityFormValues());

  useEffect(() => {
    if (activityId) {
      loadActivity(activityId).then((activity: IActivityForm) => {
        setActivity(new ActivityFormValues(activity));
      })
    }
  }, [loadActivity, activityId]);


  const handleFinalFormSubmit = (values: any) => {
    const DateAndTime = combineDateAndTime(values.date, values.time);
    const { date, time, ...activity } = values;
    activity.date = DateAndTime;
    if (!activity.id) {
      let newActivity = {
         ...activity,
         id: uuid()
       };
       createActivity(newActivity);
     } else {
       editActivity(activity);
     }
  };

  
  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment clearing >
          <FinalForm
            validate={validate}
            initialValues={activity}
            onSubmit={handleFinalFormSubmit}
            render={({ handleSubmit, invalid, pristine }) => (
              <Form onSubmit={handleSubmit} loading={activityStore.loadingInitial}>
                <Field
                  name="title"
                  placeholder="Title"
                  value={activity.title}
                  component={TextInput}
                />
                <Field
                  name="description"
                  placeholder="Description"
                  value={activity.description}
                  component={TextAreaInput}
                />
                <Field
                  name="category"
                  placeholder="Category"
                  options={category}
                  value={activity.category}
                  component={SelectInput}
                />
                <Form.Group widths='equal'>
                  <Field
                    component={DateInput}
                    name='date'
                    date={true}
                    placeholder='Date'
                    value={activity.date}
                  />
                  <Field
                    component={DateInput}
                    name='time'
                    time={true}
                    placeholder='Time'
                    value={activity.time}
                  />
                </Form.Group>
                <Field
                  name="city"
                  placeholder="City"
                  value={activity.city}
                  component={TextInput}
                />
                <Field
                  name="venue"
                  placeholder="Venue"
                  value={activity.venue}
                  component={TextInput}
                />
                <Button
                  loading={submiting}
                  floated="right"
                  disabled={activityStore.loadingInitial || invalid || pristine}
                  positive
                  type="submit"
                  content="Submit"
                />
                <Button
                  onClick={() =>
                    history.push(
                      activity.id ? `/activity/${activity.id}` : "/activities"
                    )
                  }
                  disabled={activityStore.loadingInitial}
                  floated="right"
                  type="button"
                  content="Cancel"
                />
              </Form>
            )}
          />
        </Segment>
      </Grid.Column>
    </Grid>
  );
};

export default observer(ActivityForm);
