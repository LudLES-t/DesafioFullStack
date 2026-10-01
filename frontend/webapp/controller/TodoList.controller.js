sap.ui.define(
  [
    "sap/ui/core/mvc/Controller",
    "sap/ui/model/json/JSONModel",
    "sap/m/MessageToast",
  ],
  function (Controller, JSONModel, MessageToast) {
    "use strict";

    return Controller.extend("todoapp.controller.TodoList", {
      apiUrl: "http://localhost:5066",

      onInit: function () {
        this.page = 1;
        this.pageSize = 10;
        this.search = "";

        this.getView().setModel(
          new JSONModel({
            busy: false,
            pageText: "",
          }),
          "view",
        );

        this.loadTodos();
      },

      loadTodos: async function () {
        const viewModel = this.getView().getModel("view");

        viewModel.setProperty("/busy", true);

        try {
          const url =
            `${this.apiUrl}/todos?page=${this.page}` +
            `&pageSize=${this.pageSize}` +
            `&title=${encodeURIComponent(this.search)}`;

          const response = await fetch(url);
          const data = await response.json();

          this.getView().setModel(new JSONModel(data), "todos");

          this.totalPages = data.totalPages;

          viewModel.setProperty(
            "/pageText",
            `Página ${data.page} de ${data.totalPages}`,
          );
        } finally {
          viewModel.setProperty("/busy", false);
        }
      },

      onSearch: function (event) {
        clearTimeout(this.searchTimer);

        const value = event.getParameter("newValue");

        this.searchTimer = setTimeout(() => {
          this.search = value;
          this.page = 1;
          this.loadTodos();
        }, 400);
      },

      onPrevious: function () {
        if (this.page > 1) {
          this.page--;
          this.loadTodos();
        }
      },

      onNext: function () {
        if (this.page < this.totalPages) {
          this.page++;
          this.loadTodos();
        }
      },

      onDetails: function (event) {
        const context = event.getSource().getBindingContext("todos");

        const id = context.getProperty("id");

        this.getOwnerComponent().getRouter().navTo("detail", { id });
      },

      onCompletedChange: async function (event) {
        const checkbox = event.getSource();

        const context = checkbox.getBindingContext("todos");

        const id = context.getProperty("id");

        const completed = event.getParameter("selected");

        const response = await fetch(`${this.apiUrl}/todos/${id}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            completed,
          }),
        });

        if (!response.ok) {
          const error = await response.json();

          MessageToast.show(error.message);

          checkbox.setSelected(!completed);
          return;
        }

        MessageToast.show("Tarefa atualizada");
        this.loadTodos();
      },
    });
  },
);
