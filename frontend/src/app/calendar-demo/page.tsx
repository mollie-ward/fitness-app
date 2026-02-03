/**
 * Workout Calendar Demo Page
 */

import { WorkoutCalendar } from '@/components/calendar';

export default function CalendarDemoPage() {
  const handleStartWorkout = (workoutId: string) => {
    console.log('Starting workout:', workoutId);
    // In production, this would navigate to the workout execution screen
  };

  return (
    <div className="container mx-auto px-4 py-8 max-w-7xl">
      <div className="mb-6">
        <h1 className="text-4xl font-bold text-gray-900 mb-2">Workout Calendar Demo</h1>
        <p className="text-gray-600">
          Interactive calendar component showing your training plan with multiple view modes.
        </p>
      </div>

      <WorkoutCalendar
        userId="demo-user"
        onStartWorkout={handleStartWorkout}
        enableKeyboardNav={true}
      />

      <div className="mt-12 p-6 bg-gray-50 rounded-lg">
        <h2 className="text-2xl font-semibold mb-4">Features Demonstrated</h2>
        <div className="grid md:grid-cols-2 gap-6">
          <div>
            <h3 className="font-semibold text-lg mb-2">Views</h3>
            <ul className="list-disc list-inside space-y-1 text-gray-700">
              <li>Daily view with today&apos;s workout highlighted</li>
              <li>Weekly view showing 7-day schedule</li>
              <li>Monthly calendar grid with workout density</li>
            </ul>
          </div>
          <div>
            <h3 className="font-semibold text-lg mb-2">Discipline Colors</h3>
            <ul className="list-disc list-inside space-y-1 text-gray-700">
              <li>HYROX - Orange/Red</li>
              <li>Running - Blue</li>
              <li>Strength - Purple/Green</li>
              <li>Hybrid - Gradient</li>
              <li>Rest - Gray</li>
            </ul>
          </div>
          <div>
            <h3 className="font-semibold text-lg mb-2">Status Indicators</h3>
            <ul className="list-disc list-inside space-y-1 text-gray-700">
              <li>Completed - Green checkmark</li>
              <li>Scheduled - Calendar icon</li>
              <li>Missed - Red X</li>
              <li>In Progress - Yellow play icon</li>
            </ul>
          </div>
          <div>
            <h3 className="font-semibold text-lg mb-2">Accessibility</h3>
            <ul className="list-disc list-inside space-y-1 text-gray-700">
              <li>Keyboard navigation (arrow keys)</li>
              <li>Screen reader support</li>
              <li>WCAG AA compliant colors</li>
              <li>Proper ARIA labels</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}
