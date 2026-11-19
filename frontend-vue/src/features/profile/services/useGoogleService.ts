import { useAuthStore } from '@/stores/auth';
import { getGoogleAuthLinkRedirect, postGoogleAuthLink } from '@/api/google-auth';
import { useQuery, useMutation } from '@tanstack/vue-query';
import { computed } from 'vue';
import type { ProfileDto } from '@/api/generated/models';

export function useGoogleService() {
  const authStore = useAuthStore();
  const isGoogleConnected = computed(() => authStore.user?.isGoogleConnected ?? false);

  const handleGoogleLogin = () => {
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