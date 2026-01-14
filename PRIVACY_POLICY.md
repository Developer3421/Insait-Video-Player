# Privacy Policy — Insait Video Player

Effective date: 2026-01-14

Insait Video Player (“the app”) is a local media player for Windows. This policy explains what data the app stores, how it’s used, and what choices you have.

## 1) Summary (plain language)
- The app is designed to work offline and **does not require an account**.
- The app stores **playback history** and **session restore** data **locally on your device**.
- File paths saved in history/session are **encrypted**. The encryption key is protected using **Windows DPAPI (CurrentUser scope)**.
- The app does not sell your data.

## 2) Data the app stores
The app may store the following information **on your device**:

### A) Playback history
To show your recent items and let you continue watching, the app can store:
- Video file path (stored **encrypted**)
- Date/time last watched
- Last playback position/time

### B) Session restore
To restore your tabs and settings after restarting, the app can store:
- Open tabs list (file path stored **encrypted**, if present)
- Active tab order
- Volume level
- Last saved time
- Selected audio output device ID (if chosen)
- Selected language code (UI language)

### C) App files
Local app data can include:
- A local database file (LiteDB)
- A local key file used to protect/decrypt stored file paths

## 3) Data the app does *not* collect
- No account credentials
- No contacts, calendars, or messages
- No precise location data
- No advertising identifier

## 4) How the app uses stored data
The app uses the locally stored data only to:
- Show playback history
- Restore your previous session (tabs)
- Remember playback position and selected settings (e.g., volume, audio device)

## 5) Sharing and third parties
### A) Sharing
The app **does not transmit** your history/session data to the developer.

### B) Third‑party components
The app uses third‑party libraries (for example, **LibVLC**) to play media. These components run locally as part of the app.

If you choose to open network media (for example, an internet stream), network requests may be made to the address you provide, as required to play that media.

## 6) Storage, security, and encryption
- History/session data is stored locally using **LiteDB**.
- File paths stored in the database are encrypted using **AES**.
- The AES key is protected using **Windows DPAPI** with **CurrentUser** scope (meaning it’s intended to be accessible only to the same Windows user account on the same device).

Note: No security method is perfect. If your Windows account or device is compromised, local app data may be accessible.

## 7) Changes to this policy
We may update this policy from time to time. The “Effective date” at the top will reflect the latest version.

## 8) Contact
If you have questions about this Privacy Policy, contact the developer via the contact information provided in the Microsoft Store listing for Insait Video Player.
