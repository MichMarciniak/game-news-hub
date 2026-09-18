import { setAccessToken } from "@/api/custom-instance";
import { postLogin, usePostLogin, usePostLogout, usePostLogoutAll } from "@/api/generated";
import { type LoginDto, type ProfileDto } from "@/api/generated/models";
import { defineStore } from "pinia";
import { computed, ref } from "vue";


export const useAuthStore = defineStore('auth', () => {
  const user = ref<ProfileDto | null>(null)
  const token = ref<string | null>(null)
  
  const isAuthenticated = computed(() => !!token.value)

  const login = async (values : LoginDto) => {
    try {
      const data = await postLogin(values);
      // user.value = data.data.user
    } catch (error) {
      return 'Invalid username or password'
    }

  }

  const logout = () => {
    user.value = null
    token.value = null

    usePostLogout()
  }

  const logoutAll = () => {
    user.value = null
    token.value = null

    usePostLogoutAll()
  }

  return {user, token, isAuthenticated, login, logout, logoutAll}
})