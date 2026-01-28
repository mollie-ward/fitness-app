import type { Metadata } from 'next';
import './globals.css';
import { Navigation } from '@/components/layout/navigation';
import { ErrorBoundary } from '@/components/layout/error-boundary';

export const metadata: Metadata = {
  title: 'FitnessApp - AI-Powered Personal Training',
  description: 'Personalized fitness coaching powered by AI',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="font-sans antialiased">
        <ErrorBoundary>
          <Navigation />
          <main className="min-h-[calc(100vh-4rem)]">{children}</main>
        </ErrorBoundary>
      </body>
    </html>
  );
}
