/**
 * Step 1: Welcome & Introduction
 */
import React from 'react';
import Image from 'next/image';
import { CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

interface WelcomeStepProps {
  onNext: () => void;
}

export function WelcomeStep({ onNext }: WelcomeStepProps) {
  return (
    <>
      <CardHeader>
        <div className="flex flex-col items-center text-center">
          <div className="mb-6 relative w-32 h-32 rounded-full overflow-hidden border-4 border-blue-600">
            <Image
              src="/coachTom.png"
              alt="Coach Tom"
              fill
              className="object-cover"
              priority
            />
          </div>
          <CardTitle className="text-3xl mb-2">Welcome to Your Fitness Journey!</CardTitle>
          <CardDescription className="text-lg max-w-2xl">
            Hi, I'm Coach Tom! I'll be your personal fitness coach, creating customized training 
            plans tailored to your goals, schedule, and experience level.
          </CardDescription>
        </div>
      </CardHeader>

      <CardContent className="text-center space-y-4">
        <p className="text-gray-700">
          To get started, I need to learn about your fitness background, goals, and preferences. 
          This will take about <strong>4-5 minutes</strong>.
        </p>

        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 text-left">
          <h3 className="font-semibold text-blue-900 mb-2">What we'll cover:</h3>
          <ul className="space-y-2 text-blue-800">
            <li className="flex items-start">
              <span className="mr-2">✓</span>
              <span>Your current fitness level across different disciplines</span>
            </li>
            <li className="flex items-start">
              <span className="mr-2">✓</span>
              <span>Your training goals and target dates</span>
            </li>
            <li className="flex items-start">
              <span className="mr-2">✓</span>
              <span>Your weekly training availability</span>
            </li>
            <li className="flex items-start">
              <span className="mr-2">✓</span>
              <span>Any injuries or limitations (optional)</span>
            </li>
            <li className="flex items-start">
              <span className="mr-2">✓</span>
              <span>Your training background and experience</span>
            </li>
          </ul>
        </div>

        <Button 
          onClick={onNext}
          size="lg"
          className="mt-6 px-8"
        >
          Let's Get Started! →
        </Button>
      </CardContent>
    </>
  );
}
