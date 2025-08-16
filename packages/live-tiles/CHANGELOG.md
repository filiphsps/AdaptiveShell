# @metro-ui/live-tiles

## 0.1.0

### Minor Changes

- feat: Add documentation generation and publishing

### Patch Changes

- chore: Migrate the expo app from the root into the apps/launcher
  directory, enable composite projects, update dependencies, add some
  tests, migrate to using turbo for task running, add changeset support
  with some experimental conventional commits support with it, add
  commitlint, finally fix a bunch of minor issues here and there.

    With this commit, we're finally off to the races!

- fix(live-tiles): Only include the built files as a part of the package

    Previously we accidentally included source files,
    config files, assets, etc, with the built package;
    we now no longer waste data like that.

- chore(deps): update typescript-eslint monorepo to v8.39.1

- chore(deps): update dependency @types/node to v24.2.0

- chore(deps): update dependency @types/node to v24.2.1

- chore(deps): update typescript-eslint monorepo to v8.39.0

- chore(deps): update dependency eslint-plugin-prettier to v5.5.4

- docs: Add `jsdoc` to two live-tile classes

- chore(deps): update eslint monorepo to v9.33.0

- chore: Try and fix the `release` job

- chore(deps): update dependency @types/node to v24.3.0

- chore(deps): update dependency ts-jest to v29.4.1
