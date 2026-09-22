// theme/presets.ts
import { definePreset, palette } from '@openvue/themes';
import Aura from '@openvue/themes/aura';

type PresetDefinition = {
  color: string;
  preset: ReturnType<typeof definePreset>;
};

export const presets = {
  blue: {
    color: '#3B82F6',
    preset: definePreset(Aura, {
      semantic: { primary: palette('#3B82F6') }
    }),
  },
  green: {
    color: '#10B981',
    preset: definePreset(Aura, {
      semantic: { primary: palette('#10B981') }
    }),
  },
  violet: {
    color: '#8B5CF6',
    preset: definePreset(Aura, {
      semantic: { primary: palette('#8B5CF6') }
    }),
  },
} satisfies Record<string, PresetDefinition>;

export type PresetName = keyof typeof presets;