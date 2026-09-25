<script setup lang="ts">
withDefaults(defineProps<{
  name: string
  placeholder?: string
}>(), {
  placeholder: undefined
})
</script>

<template>
  <Password
    :name="name"
    :placeholder="placeholder"
    toggleMask
    fluid
    variant="filled"
    :feedback="true"
  >
    <template #header>
      <div class="font-semibold text-xs mb-2">Strength</div>
    </template>

    <template #footer>
      <div class="mt-2 pt-2 border-t">
        <p class="text-xs font-semibold mb-1">Password requirements:</p>
        <ul class="text-xs list-disc pl-4 space-y-1">
          <li>Minimum 8 characters</li>
          <li>At least one uppercase</li>
          <li>At least one number</li>
        </ul>
      </div>
    </template>
  </Password>
</template>