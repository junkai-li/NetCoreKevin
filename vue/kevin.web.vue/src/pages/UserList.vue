<template>
  <UserManagement ref="userManagementRef" />
</template>

<script setup>
/* eslint-disable no-undef */
import UserManagement from "../components/UserManagement.vue";
import { ref } from "vue";

const userManagementRef = ref();
</script>

<style scoped>
@import "../css/MyTable.css";
@import "../css/UserList.css";  
@import "../css/management.css";  
</style>