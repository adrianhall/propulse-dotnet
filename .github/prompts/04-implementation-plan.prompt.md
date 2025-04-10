Generate an implementation plan based on the technical specification document provided to you.  Before generating the implementation plan, be sure to review the docs/project-proposal.md file to understand an overview of the project.

Provide an incremental implementation plan.  Each step of the plan should leave the project in a place where it can be compiled and include relevant unit and integration tests.

Each step of the implementation plan should be written as Markdown in a separate file (e.g. project/phase-1/implementation-plan/01-coreprojects.md) as if it were being given to a different developer to complete.  Write the implementation plan as if it were a GitHub issue, including a title, short aim, detailed steps to complete (without writing any C# code), and a definition of done.

Rules:

- Assume a mid-career developer familiar in C# and ASP.NET Core development is going to execute the step and provide the implementation of the step as a git commit.
- Do not generate code - however, indicate the projects affected and potential class diagrams.
- Include the objective of the step and the steps to achieve that objective.
- Include a definition of "done", which should include documentation and testing requirements.
- Generate Mermaid diagrams for class relationships where it would make the implementation plan easier to understand.
- Suggest common design patterns and best practices that may be used in completing the implementation.