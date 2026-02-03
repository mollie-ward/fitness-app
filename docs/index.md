# Spec2Cloud Documentation

This folder holds the long-form documentation that used to live in the root README.

- Start here: if you’re new, read the root [README](../README.md) first.

## Topics

- [Shell baselines (Greenfield)](shells.md)
- [Architecture (Dev Container, MCP, Agents)](architecture.md)
- [Workflows (Greenfield + Brownfield)](workflows.md)
- [Generated documentation structure (`specs/` layout)](specs-structure.md)
- [Managing standards with APM](apm.md)
- [Example usage](examples.md)
- [Key benefits](benefits.md)

## Related guides

- [Integration & installation guide](../INTEGRATION.md)

## Infrastructure & Deployment

- **[Quick Start Guide](quickstart-infrastructure.md)** - Get running locally in 5 minutes
- **[Deployment Guide](deployment-guide.md)** - Complete Azure deployment instructions
- **[Security Checklist](security-checklist.md)** - Security best practices and verification
- [Infrastructure README](../infrastructure/README.md) - Bicep templates and architecture
- [GitHub Secrets Template](../infrastructure/github-secrets-template.md) - CI/CD secrets setup

---

## Component Documentation

### Workout Calendar UI Component
Complete architectural guidance for implementing the workout calendar feature:

**Quick Start - Read in Order:**
1. **[Architecture Summary](./workout-calendar-architecture-summary.md)** ⭐ Start Here
   - Overview, roadmap, and implementation checklist
2. **[Implementation Guide](./workout-calendar-implementation-guide.md)**
   - Code examples and patterns
3. **[Architecture Diagram](./workout-calendar-architecture-diagram.md)**
   - Visual component relationships and data flow
4. **[ADR-0001](../specs/adr/0001-workout-calendar-component-architecture.md)**
   - Detailed architectural decisions

### Architecture Decision Records
- **[ADR Index](../specs/adr/README.md)** - All architectural decisions
- **[ADR-0001: Workout Calendar Architecture](../specs/adr/0001-workout-calendar-component-architecture.md)**

---

## Architecture Documentation

- **[Frontend Architecture](./frontend-architecture.md)** - Next.js, React, TypeScript patterns
- **[Backend Architecture](./backend-architecture.md)** - .NET Core, ASP.NET, API design
- **[Backend Scaffolding Summary](./backend-scaffolding-summary.md)** - .NET project setup
- **[Exercise Database Implementation](./exercise-database-implementation.md)** - Exercise data model
