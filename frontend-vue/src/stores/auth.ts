import { setUnauthorizedHandler } from "@/api/custom-instance";
import { getUsersMe, usePostAuthLogin, usePostAuthLogout, usePostAuthLogoutAll } from "@/api/generated";
import { type LoginDto, type ProfileDto } from "@/api/generated/models";
import { defineStore } from "pinia";
import { computed, ref } from "vue";


export const useAuthStore = defineStore('auth', () => {
  const user = ref<ProfileDto | null>(null)
  const isInitialized = ref(false)
  
  const isAuthenticated = computed(() => user.value !== null)
  const isAdmin = computed(() => user.value?.isAdmin)
  const isSteamConnected = computed(() => user.value?.steamId !== null)

  const clear = () => {user.value = null}
  setUnauthorizedHandler(clear)

  const {mutateAsync: loginMutate} = usePostAuthLogin()
  const {mutate: logoutMutate} = usePostAuthLogout()
  const {mutate: logoutAllMutate} = usePostAuthLogoutAll()

  const initialize = async () => {
    if (isInitialized.value) return // dodatkowe zabezpieczenie, na wypadek podwójnego wywołania

    try {
      user.value = await getUsersMe()
    } catch {
      user.value = null
    } finally {
      isInitialized.value = true
    }
  }

  const login = async (values: LoginDto) => {
    user.value = await loginMutate({data: values})
  }

  // clear tanstack cache on logout???
  const logout = () => logoutMutate(undefined, {onSettled: clear })
  const logoutAll = () => logoutAllMutate(undefined, {onSettled: clear })

  return {user, isAuthenticated, isAdmin, isSteamConnected, login, logout, logoutAll, initialize}
})