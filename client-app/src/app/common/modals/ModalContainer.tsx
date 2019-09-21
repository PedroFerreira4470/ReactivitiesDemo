import React, { useContext } from "react";
import { Modal } from "semantic-ui-react";
import { RootStoreContext } from "../../stores/rootStore";
import { observer } from "mobx-react-lite";

const ModalContainer = () => {
     const rootStore = useContext(RootStoreContext);
     const {modal,closeModal} = rootStore.modalStore;
  return (
    <Modal open={modal.open} onClose={closeModal} size='mini'>
      <Modal.Content>{modal.body}</Modal.Content>
    </Modal>
  );
};

export default observer(ModalContainer);
