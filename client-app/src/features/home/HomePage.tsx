import React, { useContext, Fragment } from "react";
import { Container, Segment, Header, Button, Image } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { RootStoreContext } from "../../app/stores/rootStore";
import LoginForm from "../user/LoginForm";
import RegisterForm from "../user/RegisterForm";

const HomePage = () => {
  const useStore = useContext(RootStoreContext);
  const { isLoggedIn, user } = useStore.userStore;
  const { openModal } = useStore.modalStore;
  return (
    <Segment inverted textAlign="center" vertical className="masthead">
      <Container text>
        <Header as="h1" inverted>
          <Image
            size="massive"
            src="/assets/logo.png"
            alt="logo"
            style={{ marginBottom: 12 }}
          />
          Reactivities
        </Header>
        {isLoggedIn && user ? (
          <Fragment>
            <Header
              as="h2"
              inverted
              content={`Welcome Back ${user.displayName}`}
            />
            <Button as={Link} to="/activities" size="huge" inverted>
              Go To Activities!
            </Button>
          </Fragment>
        ) : (
          <Fragment>
            <Header as="h2" inverted content="Welcome to Reactivities" />
            <Button onClick={() => openModal(<LoginForm/>)} size="huge" inverted>
              Log in
            </Button>
            <Button onClick={() => openModal(<RegisterForm/>)} size="huge" inverted>
              Sign up
            </Button>
          </Fragment>
        )}
      </Container>
    </Segment>
  );
};

export default HomePage;
