# RedMenu

A hopeful fork.

The [original RedMenu](https://github.com/TomGrobbe/RedMenu) by TomGrobbe hasn't been touched in years. So I am gonna try to work on it.

All credit for the original code goes to Tom. His README said code from the repo could be reused as long as it's credited, so that's what this is.

## What is it

A server-sided admin/trainer menu for RedM. Permissions are handled through FiveM's ACE system, so server owners decide exactly who gets access to what.

## Installing

1. Grab the latest release, drop it into `resources/[local]/RedMenu/`
2. Name the folder `RedMenu` exactly, it won't work otherwise
3. In your `server.cfg`, add (in this order):
   ```
   exec resources/[local]/RedMenu/config.cfg
   start RedMenu
   ```
4. Set up permissions (see below)
5. Restart, check console for errors

## Permissions

Every feature maps to an ACE permission like `RedMenu.<Category>.<Permission>`, e.g:

```cfg
add_ace group.admin RedMenu.PlayerMenu.GodMode allow
add_ace group.admin RedMenu.WeaponsMenu.InfiniteAmmo allow
add_ace group.admin RedMenu.* allow
```
