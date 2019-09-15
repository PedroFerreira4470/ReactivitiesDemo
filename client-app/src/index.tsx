import React from "react";
import ReactDOM from "react-dom";
import { Router } from "react-router-dom";
import "./app/layout/style.css";
import App from "./app/layout/App";
import * as serviceWorker from "./serviceWorker";
import ScrollToTop from "./app/layout/ScrollToTop";
import { createBrowserHistory } from "history";
import 'react-toastify/dist/ReactToastify.min.css';

export const history = createBrowserHistory();
ReactDOM.render(
  <Router history={history}>
    <ScrollToTop>
      <App />
    </ScrollToTop>
  </Router>,
  document.getElementById("root")
);

serviceWorker.unregister();
