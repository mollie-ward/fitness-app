# 📝 Product Requirements Document (PRD)

## 1. Purpose

This product is a mobile fitness application designed as a value-added service for gym members, providing personalized coaching and adaptive training plans through an AI-powered conversational coach. The app solves the problem of generic, static workout plans by delivering dynamic, personalized training that adapts to each user's schedule, fitness level, goals, and life circumstances—replicating the experience of having a dedicated human coach.

**Target Users:**
- Gym members seeking structured, goal-oriented training
- Athletes preparing for specific events (e.g., HYROX competitions)
- Individuals who want expert guidance but cannot afford or access personal trainers
- Fitness enthusiasts across beginner, intermediate, and advanced levels

## 2. Scope

### In Scope:
- Personalized training plan generation across three disciplines: HYROX, running, and strength training
- User onboarding with comprehensive fitness profiling
- Adaptive plan adjustment based on user feedback, constraints, and progress
- Conversational AI coach ("Coach Tom") for guidance, motivation, and plan modifications
- Workout calendar and scheduling functionality
- Progress tracking and completion indicators
- Injury consideration and workout adaptation
- Multi-week plan progression with goal-based timelines

### Out of Scope:
- Nutrition planning and meal tracking
- Social features (sharing workouts, following friends, leaderboards)
- Integration with wearable devices or fitness trackers (initial release)
- Video demonstrations of exercises (initial release)
- Live coaching or video calls with human trainers
- Gym facility booking or class scheduling
- Payment processing for gym memberships
- Music streaming or workout playlists

## 3. Goals & Success Criteria

### Business Goals:
- Increase gym membership retention by providing unique value-added service
- Differentiate the gym from competitors through AI-powered personalization
- Reduce demand for expensive 1-on-1 personal training while maintaining member satisfaction
- Create scalable coaching solution that serves unlimited members simultaneously

### User Goals:
- Receive expert-level training guidance without paying for personal trainers
- Achieve specific fitness goals efficiently and safely
- Maintain training consistency despite life's unpredictability
- Understand the "why" behind their workouts
- Feel supported and motivated throughout their fitness journey

### Success Metrics:
- **Engagement:** 70%+ of users complete at least 3 workouts per week
- **Retention:** 60%+ of users remain active after 90 days
- **Adaptation Rate:** Users interact with Coach Tom to modify plans at least once per month
- **Plan Completion:** 50%+ of planned weekly workouts are marked complete
- **User Satisfaction:** Net Promoter Score (NPS) of 40+
- **Goal Achievement:** 40%+ of users with dated goals report achieving them on time

## 4. High-Level Requirements

- **[REQ-1]** The app must generate personalized training plans based on user-provided fitness level, training background, goals, schedule availability, and injury considerations
- **[REQ-2]** Plans must accommodate three training disciplines: HYROX training, running training, and strength/weight training
- **[REQ-3]** The system must fit training plans exactly into user-specified available training days per week
- **[REQ-4]** Plans must be dynamic and adapt automatically based on user interactions, missed workouts, injuries, and feedback
- **[REQ-5]** Users must be able to converse naturally with an AI coach (Coach Tom) to ask questions, modify plans, report constraints, and receive guidance
- **[REQ-6]** The AI coach must be able to explain workout rationale, provide motivation, and adjust plans in response to user needs
- **[REQ-7]** The app must clearly display today's workout, the current week's plan, and upcoming key sessions
- **[REQ-8]** Users must be able to mark workouts as complete and track progress over time
- **[REQ-9]** The system must respect injury constraints by modifying or substituting workouts appropriately
- **[REQ-10]** For users with time-bound goals, the system must scale intensity and progression to align with the target date
- **[REQ-11]** The user interface must visually distinguish between the three training disciplines
- **[REQ-12]** The AI coach (Coach Tom) must have a consistent visual avatar throughout the app experience

## 5. User Stories

```gherkin
As a beginner gym member, I want to provide my fitness level during onboarding, so that I receive workouts appropriate for my current abilities.
```

```gherkin
As a HYROX competitor, I want to set a competition date as my end goal, so that my training plan builds toward peak performance at the right time.
```

```gherkin
As a busy professional, I want to specify exactly which days I can train, so that I never receive workouts on days I'm unavailable.
```

```gherkin
As a user who just sustained an injury, I want to tell Coach Tom about my limitation, so that my upcoming workouts are adjusted to avoid aggravating the injury.
```

```gherkin
As someone new to structured training, I want to ask Coach Tom why a particular workout is in my plan, so that I understand how it contributes to my goals.
```

```gherkin
As a motivated user, I want to tell Coach Tom that this week felt easy, so that future weeks increase in intensity appropriately.
```

```gherkin
As a user who missed yesterday's workout, I want to ask Coach Tom what to do, so that I can stay on track without falling behind.
```

```gherkin
As a visual learner, I want to see a calendar view of my training plan, so that I can mentally prepare for the week ahead.
```

```gherkin
As a goal-oriented user, I want to see how many planned workouts I've completed, so that I can measure my consistency and commitment.
```

```gherkin
As a returning user, I want Coach Tom to greet me and reference my progress, so that I feel supported and accountable like I would with a real coach.
```

```gherkin
As a user training across multiple disciplines, I want to clearly see whether today's workout is running, HYROX, or strength, so that I can prepare the right equipment and mindset.
```

```gherkin
As a user with changing circumstances, I want to add or remove training days mid-plan, so that my plan remains realistic and achievable.
```

## 6. Assumptions & Constraints

### Assumptions:
- Users have active gym memberships and access to basic fitness equipment (weights, cardio equipment)
- Users are capable of performing exercises independently without in-person supervision
- Users will honestly report their fitness level and provide accurate onboarding information
- Users have smartphones with sufficient storage and internet connectivity
- The AI coach can reasonably replicate key aspects of human coaching through natural language processing
- Users prefer convenience and flexibility over rigid, prescriptive training programs
- Coach Tom's avatar will be provided as a ready-to-use visual asset

### Constraints:
- The app must be delivered as an add-on to existing gym memberships, not a standalone product
- Plans must work within user-defined weekly schedules (no assumption of daily availability)
- The system must function for users across a wide spectrum of fitness levels (beginner to advanced)
- Plans must remain coherent and safe even when frequently modified by users
- The app must handle simultaneous training across three different disciplines without creating conflicts or overtraining
- User data privacy and security must comply with applicable regulations (GDPR, HIPAA where applicable)
- The app must be usable without integration to external fitness devices in the initial release

### Open Questions:
- What is the minimum viable timeframe for achieving meaningful results (e.g., 4 weeks, 8 weeks, 12 weeks)?
- Should users be limited in how frequently they can make major plan adjustments (to prevent plan incoherence)?
- How should the system handle conflicting user requests (e.g., "make it easier" followed by "I want faster progress")?
- Should there be guardrails preventing users from overtraining when requesting additional workout days?
- What happens if a user abandons their plan for multiple weeks—should they restart or resume?
- Should Coach Tom's personality be configurable (e.g., more motivational vs. more technical)?
- What level of exercise detail should be provided (sets, reps, weights, rest periods, tempo)?

---

**Document Status:** Draft v1.0  
**Last Updated:** January 28, 2026  
**Owner:** Product Manager  
**Next Review:** Pending stakeholder feedback