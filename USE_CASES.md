# Use Case Diagram — Game News Hub

Format: PlantUML.  
Stereotype `<<invoke>>` means that one use case triggers another (e.g. view game details → follow game).  
Actors: **Guest**, **User**, **Admin**, **System** (scheduled/time-triggered).

---

## PlantUML source

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle
skinparam usecase {
    BackgroundColor<<Guest>> #FFAAAA
    BackgroundColor<<User>> #AAFFAA
    BackgroundColor<<Admin>> #AAAAFF
    BackgroundColor<<System>> #FFFFAA
}

actor "Guest" as Guest <<Guest>>
actor "User" as User <<User>>
actor "Admin" as Admin <<Admin>>
actor "System (scheduled)" as System <<System>>

package "Authentication & Account" {
    usecase "Register account" as UC_Register <<Guest>>
    usecase "Login (email/password)" as UC_Login <<Guest>>
    usecase "Login with Google OAuth" as UC_GoogleLogin <<Guest>>
    usecase "Login with Steam OpenID" as UC_SteamLogin <<Guest>>
    usecase "Forgot password" as UC_ForgotPassword <<Guest>>
    usecase "Reset password" as UC_ResetPassword <<Guest>>
    usecase "Confirm email" as UC_ConfirmEmail <<Guest>>
    usecase "Resend confirmation email" as UC_ResendConfirmation <<Guest>>
    usecase "Logout" as UC_Logout <<User>>
    usecase "Logout all sessions" as UC_LogoutAll <<User>>
    usecase "Change password" as UC_ChangePassword <<User>>
    
    UC_GoogleLogin ..> UC_Login : <<invoke>>
    UC_SteamLogin ..> UC_Login : <<invoke>>
}

package "Games" {
    usecase "Search games" as UC_SearchGames <<Guest>>
    usecase "View game details" as UC_ViewGameDetails <<Guest>>
    usecase "View game addons" as UC_ViewGameAddons <<Guest>>
    usecase "Filter games (genre / platform)" as UC_FilterGames <<Guest>>
    usecase "Sort games" as UC_SortGames <<Guest>>
    usecase "Paginate games" as UC_PaginateGames <<Guest>>
    usecase "Follow game" as UC_FollowGame <<User>>
    usecase "Unfollow game" as UC_UnfollowGame <<User>>
    
    UC_SearchGames ..> UC_ViewGameDetails : <<invoke>>
    UC_ViewGameDetails ..> UC_FollowGame : <<invoke>>
    UC_SearchGames ..> UC_FilterGames : <<invoke>>
}

package "Events" {
    usecase "View events" as UC_ViewEvents <<Guest>>
    usecase "View event details" as UC_ViewEventDetails <<Guest>>
    usecase "Search events" as UC_SearchEvents <<Guest>>
    usecase "Follow event" as UC_FollowEvent <<User>>
    usecase "Unfollow event" as UC_UnfollowEvent <<User>>
    usecase "Reassign event to series" as UC_ReassignEventSeries <<Admin>>
    
    UC_ViewEvents ..> UC_ViewEventDetails : <<invoke>>
}

package "Profile" {
    usecase "View profile" as UC_ViewProfile <<User>>
    usecase "View full profile" as UC_ViewFullProfile <<User>>
    usecase "Edit profile settings" as UC_EditSettings <<User>>
    
    UC_ViewProfile ..> UC_ViewFullProfile : <<invoke>>
}

package "Wishlist" {
    usecase "View wishlist" as UC_ViewWishlist <<User>>
    usecase "Add game to wishlist" as UC_AddToWishlist <<User>>
    usecase "Remove game from wishlist" as UC_RemoveFromWishlist <<User>>
    
    UC_ViewWishlist ..> UC_AddToWishlist : <<invoke>>
    UC_ViewWishlist ..> UC_RemoveFromWishlist : <<invoke>>
}

package "Steam Integration" {
    usecase "Connect Steam account" as UC_ConnectSteam <<User>>
    usecase "Disconnect Steam account" as UC_DisconnectSteam <<User>>
    usecase "Import Steam wishlist" as UC_ImportSteamWishlist <<User>>
    usecase "Import Steam owned games" as UC_ImportSteamGames <<User>>
    usecase "Batch follow Steam games" as UC_BatchFollowSteam <<User>>
    
    UC_ImportSteamGames ..> UC_BatchFollowSteam : <<invoke>>
    UC_ConnectSteam ..> UC_SteamLogin : <<invoke>>
}

package "Google Integration" {
    usecase "Connect Google account" as UC_ConnectGoogle <<User>>
    usecase "Disconnect Google account" as UC_DisconnectGoogle <<User>>
    
    UC_ConnectGoogle ..> UC_GoogleLogin : <<invoke>>
}

package "Admin" {
    usecase "Admin panel / dashboard" as UC_AdminPanel <<Admin>>
    usecase "Search admin series" as UC_SearchSeries <<Admin>>
    usecase "Reassign platform to group" as UC_ReassignPlatformGroup <<Admin>>
    usecase "Reassign event to series" as UC_ReassignEventSeries <<Admin>>
    usecase "Manage users" as UC_ManageUsers <<Admin>>
    usecase "Manage games" as UC_ManageGames <<Admin>>
    usecase "Manage events" as UC_ManageEvents <<Admin>>
    usecase "Manage genres" as UC_ManageGenres <<Admin>>
    usecase "Manage platforms" as UC_ManagePlatforms <<Admin>>
}

package "System / Scheduled" {
    usecase "Discover events (background worker)" as UC_DiscoverEvents <<System>>
    usecase "Hydrate events with games (background worker)" as UC_HydrateEvents <<System>>
    usecase "Count follows (background job)" as UC_CountFollows <<System>>
    usecase "Sync genres (on demand / startup)" as UC_SyncGenres <<System>>
    usecase "Sync platforms (on demand / startup)" as UC_SyncPlatforms <<System>>
    usecase "Import games by external ID (IGDB/Steam)" as UC_ImportGamesByExternalId <<System>>
    
    UC_DiscoverEvents ..> UC_HydrateEvents : <<invoke>>
    UC_HydrateEvents ..> UC_ImportGamesByExternalId : <<invoke>>
}

' Actor inheritance / privilege inclusion
User ..> Guest : <<include>>
Admin ..> User : <<include>>

@enduml
```

---

## Actor privilege matrix

| Actor | Can do |
|-------|--------|
| **Guest** | Browse games & events, search, filter (genre/platform), sort, paginate, view game/event details and addons, register, login (email / Google / Steam), password reset flow |
| **User** | Everything Guest can + follow/unfollow games & events, wishlist add/remove, profile view & settings edit, connect/disconnect Steam & Google accounts, import Steam wishlist & owned games, batch-follow Steam games, logout / logout-all, change password |
| **Admin** | Everything User can + admin panel, series search, reassign platform to group, reassign event to series, manage users / games / events / genres / platforms |
| **System (scheduled)** | Discover events, hydrate events with games, count follows, sync genres/platforms, import games by external ID |

---

## Package breakdown

### Authentication & Account
- Register account (Guest)
- Login email/password (Guest)
- Login with Google OAuth (Guest) → `<<invoke>>` Login
- Login with Steam OpenID (Guest) → `<<invoke>>` Login
- Forgot / Reset password (Guest)
- Confirm / Resend confirmation email (Guest)
- Logout / Logout all sessions (User)
- Change password (User)

### Games
- Search games (Guest) → `<<invoke>>` View game details
- View game details (Guest) → `<<invoke>>` Follow game
- View game addons (Guest)
- Filter games by genre/platform (Guest)
- Sort games (Guest)
- Paginate games (Guest)
- Follow / Unfollow game (User)

### Events
- View events (Guest) → `<<invoke>>` View event details
- View event details (Guest)
- Search events (Guest)
- Follow / Unfollow event (User)
- Reassign event to series (Admin)

### Profile
- View profile (User) → `<<invoke>>` View full profile
- Edit profile settings (User)

### Wishlist
- View wishlist (User) → `<<invoke>>` Add / Remove game from wishlist
- Add game to wishlist (User)
- Remove game from wishlist (User)

### Steam Integration
- Connect / Disconnect Steam account (User) → `<<invoke>>` Login with Steam
- Import Steam wishlist (User)
- Import Steam owned games (User) → `<<invoke>>` Batch follow Steam games
- Batch follow Steam games (User)

### Google Integration
- Connect / Disconnect Google account (User) → `<<invoke>>` Login with Google

### Admin
- Admin panel / dashboard (Admin)
- Search admin series (Admin)
- Reassign platform to group (Admin)
- Reassign event to series (Admin)
- Manage users / games / events / genres / platforms (Admin)

### System / Scheduled (time-triggered, no actor)
- Discover events (background worker) → `<<invoke>>` Hydrate events with games
- Hydrate events with games → `<<invoke>>` Import games by external ID
- Count follows (background job)
- Sync genres / Sync platforms (on demand or startup)
- Import games by external ID (IGDB / Steam)

---

## Notable `<<invoke>>` flows

- Search games → View game details → Follow game  
- View wishlist → Add / Remove game from wishlist  
- Import Steam owned games → Batch follow Steam games  
- Login with Google / Steam → Login (complete sign-in)  
- Discover events → Hydrate events → Import games by external ID  
- View profile → View full profile
