You are coding a part of a project, according to an implementation plan.

- Refer to the technical specification when needed:
  - project/MVP/technical-specification/01_system_architecture.md
  - project/MVP/technical-specification/02_technology_choices.md
  - project/MVP/technical-specification/03_data_model.md
  - project/MVP/technical-specification/04_api_surface.md
  - project/MVP/technical-specification/05_user_interface.md
  - project/MVP/technical-specification/06_security_implementation.md
- Create a git branch for each step. The git branch should be named `step/<step-name>`.  For example, if you are implementing step `01-project-structure.md`, you would create `step/01-project-structure`.
- Do a build and test run after completing each step.  Iterate until the build and test complete without errors.
- All source projects should be located in the 'src' directory.
- All test projects should be located in the 'test' directory.
- Use net9.0 TFM, nullable, and implicit usings for all projects.
- Use a `Directory.Build.props` to consolidate common settings.
- Use PowerShell 7 for scripting.