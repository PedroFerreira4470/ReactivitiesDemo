import React from "react";
import { AxiosResponse } from "axios";
import { Message } from "semantic-ui-react";

interface IProps {
  error: AxiosResponse;
  text?: string;
}

const ErrorMessage: React.FC<IProps> = ({ error, text }) => {
  return (
    <Message error>
      <Message.Header>{error.statusText}</Message.Header>
      {text && <Message.Content>{text}</Message.Content>}
      {error.data && Object.keys(error.data.errors).length > 0 && (
        <Message.List>
          {Object.values(error.data.errors)
            .flat()
            .map((error, index) => (
              <Message.Item key={index} > {error} </Message.Item>
            ))}
        </Message.List>
      )}
    </Message>
  );
};

export default ErrorMessage;
