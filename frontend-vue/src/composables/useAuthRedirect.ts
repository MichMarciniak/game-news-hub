export function useAuthRedirect() {
  const REDIRECT_KEY = 'auth:redirectPath'

  function saveRedirectPath(path?: string) {
    sessionStorage.setItem(REDIRECT_KEY, path || '/')
  }

  function consumeRedirectPath(): string {
    const path = sessionStorage.getItem(REDIRECT_KEY) || '/'
    sessionStorage.removeItem(REDIRECT_KEY)
    return path
  }

  function getRedirectPath(): string | null {
    return sessionStorage.getItem(REDIRECT_KEY)
  }

  function clearRedirectPath() {
    sessionStorage.removeItem(REDIRECT_KEY)
  }

  return {
    saveRedirectPath,
    consumeRedirectPath,
    getRedirectPath,
    clearRedirectPath,
  }
}