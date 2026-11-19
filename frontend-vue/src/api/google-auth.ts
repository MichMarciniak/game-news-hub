import { customInstance } from './custom-instance';
import type { ProfileDto } from './generated/models';

export interface GoogleLinkRequest {
  code: string;
}

export const getGoogleAuthLinkRedirect = (
  options?: Parameters<typeof customInstance>[1]
) => {
  return customInstance<{ url: string }>(
    { url: '/auth/google/link/redirect', method: 'GET' },
    options
  );
};

export const postGoogleAuthLink = (
  data: GoogleLinkRequest,
  options?: Parameters<typeof customInstance>[1]
) => {
  return customInstance<ProfileDto>(
    {
      url: '/auth/google/link',
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      data,
    },
    options
  );
};