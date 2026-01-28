# Feature Requirements Document (FRD)
## Injury Management & Workout Adaptation

**Feature ID:** FRD-007  
**Priority:** High (P0)  
**Status:** Draft  
**Last Updated:** January 28, 2026  
**Related PRD:** [PRD Section 4 - REQ-9](../prd.md)

---

## 1. Feature Overview

### Purpose
Enable the system to accommodate user injuries and physical limitations by intelligently modifying or substituting exercises to allow continued training while avoiding aggravation or further harm.

### Problem Statement
Injuries are inevitable in fitness. When users get injured, traditional apps provide no guidance—forcing users to either abandon their plan entirely or risk worsening the injury by continuing unsuitable workouts. Users need intelligent accommodation that keeps them training safely.

### User Value
- Users can continue training and maintaining progress despite injuries
- Reduced fear of worsening injuries through inappropriate exercise selection
- Maintained motivation and momentum rather than complete training cessation
- Trust that the system prioritizes their long-term health over short-term gains
- Alternative exercises maintain training effect for unaffected muscle groups and systems

---

## 2. Success Criteria

### Acceptance Criteria
- [ ] Users can report new injuries with affected body part/movement pattern
- [ ] System identifies upcoming workouts that could aggravate reported injury
- [ ] Contraindicated exercises are automatically removed or modified
- [ ] Alternative exercises are provided that maintain training stimulus for unaffected areas
- [ ] Users receive clear explanation of what changed and why
- [ ] Users can update injury status (healing progress, new limitations, full recovery)
- [ ] System supports both acute injuries (recent) and chronic limitations (ongoing)
- [ ] Injury accommodations integrate seamlessly with overall plan progression

### Key Performance Indicators (KPIs)
- **Injury Reporting Rate:** 25%+ of users report at least one injury during their training
- **Continued Training:** 80%+ of users who report injuries continue training rather than dropping out
- **Appropriate Modifications:** <3% of users report that modified workouts still aggravated their injury
- **Recovery Tracking:** 60%+ of users who report injuries eventually mark them as recovered
- **Alternative Exercise Satisfaction:** 75%+ of users find substitute exercises acceptable

### Out of Scope (for this feature)
- Medical diagnosis or treatment recommendations
- Physical therapy protocols or rehabilitation programs
- Integration with healthcare providers or medical records
- Injury prevention prediction or risk scoring
- Biomechanical analysis or form correction
- Insurance or liability management

---

## 3. User Stories & Scenarios

### Core User Stories

**Story 1: New Injury Report**
```gherkin
As a user who just sprained my ankle,
I want to tell the app about my injury,
So that running workouts are removed or replaced until I heal.
```

**Story 2: Chronic Limitation**
```gherkin
As a user with chronic lower back issues,
I want to specify this limitation during onboarding,
So that deadlifts and similar movements are avoided or modified from the start.
```

**Story 3: Partial Recovery**
```gherkin
As a user whose shoulder is healing but not fully recovered,
I want to update my injury status to "improving",
So that the system can gradually reintroduce shoulder exercises at reduced intensity.
```

**Story 4: Full Recovery**
```gherkin
As a user whose knee has fully healed,
I want to mark the injury as resolved,
So that my plan returns to full programming without unnecessary restrictions.
```

**Story 5: Multiple Concurrent Injuries**
```gherkin
As a user dealing with both a wrist sprain and hamstring strain,
I want the system to accommodate both simultaneously,
So that I can still train safely around multiple limitations.
```

**Story 6: Understanding Modifications**
```gherkin
As a user whose workout was modified due to injury,
I want to understand which exercises were changed and why,
So that I trust the modifications and understand the rationale.
```

### User Journey Example

**Scenario:** Jordan strains his shoulder during Week 4 of his training plan.

1. **Tuesday:** Jordan feels shoulder pain after Monday's upper body workout
2. **Wednesday Morning:** Jordan opens app, taps "Chat with Coach Tom"
3. Jordan: "I hurt my shoulder yesterday. It hurts when I lift my arm overhead."
4. Coach Tom: "Sorry to hear that, Jordan. To make sure we don't aggravate it, I need to know: Is it sharp pain or dull ache? Can you do pushing movements (like push-ups) without pain?"
5. Jordan: "Sharp pain on overhead movements. Push-ups are okay."
6. Coach Tom: "Got it. I'm updating your profile and adjusting your upcoming workouts to avoid overhead pressing and pulling. You'll still be able to train chest, legs, and core safely. I recommend seeing a physio if pain persists beyond a week. Let me know when it starts feeling better!"
7. **Wednesday's Workout (Updated):**
   - Original: Overhead Press, Pull-Ups, Dips, Core
   - Modified: Chest Press, ~~Pull-Ups~~ (removed), Dips (kept), Core, Lower Body Addition (Romanian Deadlifts added to maintain volume)
8. **Week 5-6:** Jordan trains modified workouts successfully
9. **Week 7:** Jordan reports "shoulder feeling 80% better"
10. Coach Tom: "Great news! I'm reintroducing light overhead work at 50% intensity. We'll build back up gradually."
11. **Week 8:** Jordan marks injury as "fully recovered"; plan returns to normal

---

## 4. Functional Requirements

### WHAT the feature must do:

**FR-1: Injury Reporting Interface**
- Must allow users to report injuries through Coach Tom conversational interface
- Must capture: affected body part/region, type of pain, movements that cause pain
- Must support injury severity indication (mild, moderate, severe)
- Must timestamp injury report date
- Must allow reporting during onboarding (pre-existing conditions) or mid-plan (new injuries)

**FR-2: Exercise Contraindication Identification**
- Must maintain database of exercises tagged by: muscle groups, movement patterns, joint stress
- Must cross-reference reported injury with upcoming workouts to identify contraindicated exercises
- Must scan entire remaining plan (not just next workout)
- Must identify both obvious conflicts (shoulder injury → overhead press) and subtle ones (shoulder injury → heavy barbell squats due to bar position)

**FR-3: Exercise Substitution**
- Must provide alternative exercises that maintain training stimulus for unaffected areas
- Must ensure substitutes are appropriate for user's fitness level and available equipment
- Must maintain workout structure and progression (not just remove exercises leaving gaps)
- Must prioritize exercises that allow continued goal pursuit when possible

**FR-4: Workout Modification Explanation**
- Must clearly communicate which exercises were removed/modified and why
- Must explain what substitute exercises accomplish
- Must set expectations for how long modifications will remain in place

**FR-5: Injury Status Updates**
- Must allow users to update injury healing progress (worse, same, improving, healed)
- Must adjust accommodations based on status updates
- Must gradually reintroduce restricted exercises as injury heals (progressive re-loading)

**FR-6: Chronic vs. Acute Handling**
- Must differentiate temporary injuries (expected to heal) from permanent limitations
- Acute injuries trigger temporary modifications with planned progression back to normal
- Chronic limitations trigger permanent exercise alternatives

**FR-7: Multi-Injury Management**
- Must handle multiple concurrent injuries affecting different body parts
- Must ensure modified workouts don't create new imbalances or overuse patterns
- Must maintain sufficient training variety even with multiple restrictions

**FR-8: Safety Disclaimers**
- Must include clear disclaimer that app does not provide medical advice
- Must recommend professional medical evaluation for serious or persistent injuries
- Must never encourage training through significant pain

**FR-9: Injury History Tracking**
- Must maintain log of all reported injuries and their resolution
- Must make history accessible to user for review
- May inform future injury risk awareness (optional)

---

## 5. Non-Functional Requirements

### Safety
- Injury accommodations must be conservative (err on side of caution)
- System must never recommend training through sharp pain or worsening symptoms
- Modifications must prevent compensation injuries (overusing uninjured side)

### Responsiveness
- Injury reports must trigger plan updates within 3 seconds
- Modified workouts must appear in calendar immediately
- Users must be notified of changes before attempting next workout

### Accuracy
- Exercise substitutions must maintain equivalent training stimulus when possible
- Contraindication logic must catch all relevant movement conflicts
- False positives (unnecessarily restricted exercises) should be <5%

### Usability
- Injury reporting should be conversational and non-clinical (user-friendly language)
- Users should understand modifications without needing exercise science background
- Modified workouts should feel like coherent training, not "injury rehab"

---

## 6. Dependencies & Constraints

### Dependencies
- **Exercise Database:** Requires comprehensive exercise library with detailed tagging (muscle groups, joints, movement patterns)
- **FRD-003 (Conversational AI Coach):** Coach Tom is primary interface for injury reporting and updates
- **FRD-006 (Adaptive Plan Adjustment):** Injury modifications are a type of plan adaptation
- **FRD-002 (Plan Generation):** Modified plans must follow same generation principles

### Constraints
- Cannot provide medical diagnosis or clinical rehabilitation protocols
- Cannot guarantee injury safety (users may still worsen injuries by overriding recommendations)
- Must work with standard gym equipment (cannot prescribe specialized rehab tools)
- Must avoid creating liability through overly prescriptive medical guidance

### Integration Points
- **FRD-001 (User Onboarding):** Chronic injuries/limitations collected during profiling
- **FRD-003 (Coach Tom):** All injury conversations flow through conversational interface
- **FRD-006 (Adaptive Adjustment):** Injury triggers broader plan adaptation
- **FRD-004 (Calendar):** Modified workouts update calendar immediately

---

## 7. Injury Categories & Accommodations

### WHAT injuries the system handles:

**Common Injury Types:**
- **Upper Extremity:** Shoulder, elbow, wrist, hand injuries
- **Lower Extremity:** Hip, knee, ankle, foot injuries
- **Trunk:** Lower back, upper back, neck injuries
- **Systemic:** General illness, fatigue, overtraining symptoms

**Movement Pattern Restrictions:**
- **Push:** Chest press, shoulder press, dips, push-ups
- **Pull:** Rows, pull-ups, deadlifts
- **Squat:** Back squat, front squat, lunges
- **Hinge:** Deadlift, Romanian deadlift, good mornings
- **Overhead:** Military press, overhead squat, pull-ups
- **Impact:** Running, jumping, plyometrics

**Sample Substitution Logic:**

| Injury | Restricted Exercises | Substitutions |
|--------|---------------------|---------------|
| Shoulder (overhead pain) | Overhead press, pull-ups | Landmine press, inverted rows |
| Knee (squatting pain) | Back squat, lunges | Leg press, step-ups (reduced ROM) |
| Lower back | Deadlifts, heavy squats | Trap bar deadlifts, goblet squats |
| Ankle | Running, jumping | Rowing, cycling, swimming |
| Wrist | Barbell pressing | Dumbbell pressing, machine work |

---

## 8. Open Questions

1. **Rehabilitation Progression:** Should system provide structured rehab protocols, or only avoid aggravation?
2. **Medical Integration:** Should app recommend specific timeframes for medical evaluation (e.g., "see doctor if pain persists >2 weeks")?
3. **User Overrides:** Should users be able to override injury restrictions if they feel ready (with warnings)?
4. **Injury Severity Classification:** Should system ask users to rate pain on scale (1-10) to determine modification severity?
5. **Preventive Modifications:** Should system suggest exercise modifications to prevent injuries (not just react to them)?
6. **Professional Consultation:** Should app integrate with virtual PT/physio consultations for serious injuries?
7. **Insurance/Liability:** What legal disclaimers are needed to limit liability for injury-related features?

---

## 9. Acceptance Testing Scenarios

### Test Scenario 1: Acute Injury Reporting
- **Given:** User reports new knee pain affecting squatting movements
- **When:** Injury is logged via Coach Tom
- **Then:** All upcoming workouts with squats, lunges, and similar movements should be modified with alternative exercises

### Test Scenario 2: Exercise Substitution Quality
- **Given:** User has shoulder injury restricting overhead movements
- **When:** Upcoming workout includes overhead press
- **Then:** Substitute should maintain upper body pressing stimulus (e.g., landmine press, incline press)

### Test Scenario 3: Multi-Injury Accommodation
- **Given:** User reports both wrist sprain and hamstring strain
- **When:** Plan is updated
- **Then:** Workouts should avoid wrist-loading exercises AND hamstring-intensive exercises while maintaining training variety

### Test Scenario 4: Injury Recovery Progression
- **Given:** User reports shoulder injury healing progress (50% → 80% → 100%)
- **When:** Status updates are logged
- **Then:** Restricted exercises should gradually reintroduce at reduced intensity before returning to full programming

### Test Scenario 5: Chronic Limitation
- **Given:** User indicates permanent lower back limitation during onboarding
- **When:** Plan is generated
- **Then:** All future plans should exclude heavy spinal loading (deadlifts, heavy squats) permanently

---

## 10. Metrics & Success Tracking

### Injury Reporting Metrics
- **Injury Incidence Rate:** Percentage of users reporting injuries during training
- **Injury Type Distribution:** Frequency of different injury categories
- **Time to Report:** Average delay between injury occurrence and reporting

### Modification Effectiveness Metrics
- **Continued Training Rate:** Percentage of injured users who continue vs. abandon training
- **Re-Injury Rate:** Percentage of users who report worsening of same injury after modifications
- **Recovery Time:** Average time from injury report to full recovery marker

### User Satisfaction Metrics
- **Substitution Acceptance:** User feedback on quality of alternative exercises
- **Modification Clarity:** User understanding of why changes were made
- **Trust in System:** User confidence that modifications are safe and appropriate

### Safety Metrics
- **Aggravation Reports:** Instances where modified workouts still caused pain (should be <3%)
- **Medical Escalation:** Users who sought professional help after injury reporting
- **Dropout Prevention:** Retention comparison between injured users vs. non-injured cohort

---

**Document Status:** Draft v1.0  
**Next Steps:** Exercise database tagging (contraindications, muscle groups, joints), substitution algorithm development, Coach Tom injury conversation flows, medical disclaimer drafting
