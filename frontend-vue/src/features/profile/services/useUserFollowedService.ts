import { useGetUsersMeFollowedGames, useGetUsersMeFollowedEvents } from '@/api/generated'

export function useUserFollowedService() {
  const { data: followedGames, isPending: gamesPending } = useGetUsersMeFollowedGames()
  const { data: followedEvents, isPending: eventsPending } = useGetUsersMeFollowedEvents()

  return {
    followedGames,
    gamesPending,
    followedEvents,
    eventsPending
  }
}
