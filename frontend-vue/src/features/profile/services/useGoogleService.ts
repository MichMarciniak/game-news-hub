import { useAuthStore } from '@/stores/auth';
import { getGoogleAuthLogin, getGoogleAuthLinkRedirect, postGoogleAuthLink } from '@/api/google-auth';
import { useQuery, useMutation } from '@tanstack/vue-query';
import { computed, ref } from 'vue';
import type { ProfileDto } from '@/api/generated/models';

export function useGoogleService() {
  const authStore = useAuthStore();
  const googleId = computed(() => authStore.user?.googleId ?? null);

  const { data: googleLoginUrl, isPending: loginPending, refetch: refetchLoginUrl } = useQuery({
    queryKey: ['google', 'auth', 'login'],
    queryFn: () => getGoogleAuthLogin().then(res => res.url),
    enabled: true,
  });

  const { data: googleLinkUrl, isPending: linkRedirectPending } = useQuery({
    queryKey: ['google', 'auth', 'link', 'redirect'],
    queryFn: () => getGoogleAuthLinkRedirect().then(res => res.url),
    enabled: false, // Only fetch when user wants to link
  });

  const handleGoogleLogin = () => {
    if (!googleLoginUrl.value) return;
    window.location.href = googleLoginUrl.value;
  };

  const handleGoogleLink = () => {
    if (!googleLinkUrl.value) return;
    window.location.href = googleLinkUrl.value;
  };

  const linkMutation = useMutation({
    mutationFn: (code: string) => postGoogleAuthLink({ code }),
    onSuccess: () => {
      // Invalidate user data to refetch profile with GoogleId
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
    googleId,
    googleLoginUrl,
    loginPending,
    handleGoogleLogin,
    googleLinkUrl,
    linkRedirectPending,
    handleGoogleLink,
    linkGoogleAccount,
    isLinking: computed(() => linkMutation.isPending.value),
    linkError: computed(() => linkMutation.error.value?.message ?? null),
    refetchLoginUrl,
  };
}