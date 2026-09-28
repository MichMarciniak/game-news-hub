// orval.config.ts
import { defineConfig } from 'orval';

export default defineConfig({
  gameNewsHub: {
    input: {
      target: 'http://localhost:5190/openapi/v1.json', // .NET Swagger URL
    },
    output: {
      mode: 'tags-split', // Groups endpoints into feature files (events.ts, auth.ts, etc.)
      target: 'src/api/generated/endpoints.ts',
      schemas: 'src/api/generated/models', // Outputs DTO TypeScript types here
      client: 'vue-query', // Auto-generates Vue Query hooks
      httpClient: 'axios',
      override: {
        mutator: {
          path: 'src/api/custom-instance.ts',
          name: 'customInstance',
        },
        query: {
          useInfinite: true,
          useInfiniteQueryParam: 'page'
        }
      },
    },
  },
});