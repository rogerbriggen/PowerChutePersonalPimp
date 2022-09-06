# PowerChutePersonalPimp
Pimp APC PowerChute Personal for APC UPS with custom actions

# Current State
not working

# Planned features
- Execute actions on events

- Planned actions
  - Execute custom script or application
  - Send Wake up on LAN magic packets to wake another machine
  - Shutdown computer
  - Wait a custom time to trigger another action if still in this state

- Planned events
  - Switched from AC to battery
  - Switched from battery to AC
  - Error communicating with UPC

# Thanks
Thanks to https://github.com/RoelVB/powerchute-personal-notifications for publishing the event ids

# Resources
- [APC PowerChute Personal Edition](https://www.apc.com/us/en/product-range/61934-powerchute-personal-edition/)
- [Script to add notifications to PowerChute Personal](https://github.com/RoelVB/powerchute-personal-notifications)
- [Event IDs](https://community.se.com/t5/APC-UPS-Data-Center-Enterprise/Powerchute-Personal-Edition-3-0-2-Windows-Event-ID-list/m-p/311074)
- [More Event IDs](https://static.spiceworks.com/attachments/post/0017/1088/APC-Powerchute-Personal-3x.pdf)
- [Wake on LAN](https://benniroth.com/blog/2021-6-21-csharp-wake-over-lan/)
