import { useAuthStore } from '@/stores/auth';
import { postGoogleAuthLink } from '@/api/google-auth';
import { useMutation } from '@tanstack/vue-query';
import { computed } from 'vue';
import type { ProfileDto } from '@/api/generated/models';
import { useRoute } from 'vue-router';
import { useAuthRedirect } from '@/composables/useAuthRedirect';

export function useGoogleService() {
  const authStore = useAuthStore();
  const route = useRoute();
  const { saveRedirectPath } = useAuthRedirect();
  const isGoogleConnected = computed(() => authStore.user?.isGoogleConnected ?? false);

  const handleGoogleLogin = () => {
    const redirectPath = route.query.redirectPath as string | undefined
    saveRedirectPath(redirectPath)
    window.location.href = '/api/auth/google/login/redirect';
  };

  const handleGoogleLink = () => {
    window.location.href = '/api/auth/google/link/redirect';
  };

  const linkMutation = useMutation({
    mutationFn: (code: string) => postGoogleAuthLink({ code }),
    onSuccess: () => {
      authStore.initialize();
    },
  });

  const linkGoogleAccount = async (code: string): Promise<ProfileDto | null> => {
    try {
      const result = await linkMutation.mutateAsync(code);
      return result;
    } catch {
      return null;
    }
  };

  return {
    isGoogleConnected,
    handleGoogleLogin,
    handleGoogleLink,
    linkGoogleAccount,
    isLinking: computed(() => linkMutation.isPending.value),
    linkError: computed(() => linkMutation.error.value?.message ?? null),
  };
}