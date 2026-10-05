import { setAccessToken } from "@/api/custom-instance";
import { postAuthRefresh, usePostAuthLogin, usePostAuthLogout, usePostAuthLogoutAll } from "@/api/generated";
import { type LoginDto, type ProfileDto } from "@/api/generated/models";
import { defineStore } from "pinia";
import { computed, ref } from "vue";


export const useAuthStore = defineStore('auth', () => {
  const user = ref<ProfileDto | null>(null)
  const token = ref<string | null>(null)
  const isInitialized = ref(false)
  
  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.isAdmin)

  const {mutateAsync: loginMutate} = usePostAuthLogin()
  const {mutate: logoutMutate} = usePostAuthLogout()
  const {mutate: logoutAllMutate} = usePostAuthLogoutAll()

  const initialize = async () => {
    if (isInitialized.value) return // dodatkowe zabezpieczenie, na wypadek podwójnego wywołania

    try {
      const data = await postAuthRefresh()
      setAccessToken(data.accessToken as string)
      token.value = data.accessToken as string
      user.value = data.user as ProfileDto
    } catch {
      setAccessToken(null)
      user.value = null
      token.value = null
    } finally {
      isInitialized.value = true
    }
  }

  const login = async (values: LoginDto) => {
    const data = await loginMutate({data: values})
    user.value = data.user as ProfileDto
    token.value = data.accessToken as string
    setAccessToken(data.accessToken as string)
  }

  const logout = () => {
    logoutMutate(undefined, {
      onSettled: () => {
        user.value = null
        token.value = null
        setAccessToken(null)

      }
    })
  }

  const logoutAll = () => {
    logoutAllMutate(undefined, {
      onSettled: () => {
        user.value = null
        token.value = null
        setAccessToken(null)

      }
    })
  }

  return {user, token, isAuthenticated, isAdmin, login, logout, logoutAll, initialize}
})