'use client';

import { ProtectedRoute } from '@/components/auth/protected-route';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useAuthStore } from '@/lib/auth-store';

function DashboardContent() {
  const user = useAuthStore((state) => state.user);

  return (
    <div className="min-h-[calc(100vh-4rem)] bg-gray-50 px-4 py-8 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Welcome back, {user?.firstName || 'Athlete'}!
          </h1>
          <p className="mt-2 text-gray-600">
            Here&apos;s your personalized fitness dashboard
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          <Card>
            <CardHeader>
              <CardTitle>Today&apos;s Workout</CardTitle>
              <CardDescription>Your scheduled training session</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-gray-600">
                No workout scheduled for today. Visit your training plan to add workouts.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Training Plan</CardTitle>
              <CardDescription>Your personalized program</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-gray-600">
                Get started by creating your first training plan tailored to your goals.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Progress</CardTitle>
              <CardDescription>Track your achievements</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-gray-600">
                Complete workouts to see your progress and achievements here.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Coach Tom</CardTitle>
              <CardDescription>Your AI fitness coach</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-gray-600">
                Ask Coach Tom questions about your training, get motivation, or adjust your plan.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Calendar</CardTitle>
              <CardDescription>Workout schedule</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-gray-600">
                View your weekly workout schedule and upcoming training sessions.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Profile</CardTitle>
              <CardDescription>Your fitness profile</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-gray-600">
                Update your fitness level, goals, and training preferences.
              </p>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}

export default function DashboardPage() {
  return (
    <ProtectedRoute>
      <DashboardContent />
    </ProtectedRoute>
  );
}
