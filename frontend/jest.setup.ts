import '@testing-library/jest-dom';
import React from 'react';

// Mock scrollIntoView
Element.prototype.scrollIntoView = jest.fn();

// Mock react-markdown to avoid ESM issues in tests
jest.mock('react-markdown', () => {
  return {
    __esModule: true,
    default: ({ children }: { children: string }) => React.createElement('div', null, children),
  };
});

// Mock remark-gfm
jest.mock('remark-gfm', () => {
  return {
    __esModule: true,
    default: () => {},
  };
});

