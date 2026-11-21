<script setup lang="ts">
import { computed, reactive } from 'vue'
import type { SteamGameDto } from '@/api/generated/models'

const props = defineProps<{
  games: SteamGameDto[]
  visible: boolean
  isImporting: boolean
}>()

const emit = defineEmits<{
  (e: 'update:visible', val: boolean): void
  (e: 'batch-follow', ids: (number | string)[]): void
}>()

const selected = reactive(new Set<string>())

const toggleSelect = (id: string | number) => {
  const idStr = String(id)
  if (selected.has(idStr)) {
    selected.delete(idStr)
  } else {
    selected.add(idStr)
  }
}

const toggleSelectAll = () => {
  const validIds = props.games.map(g => g.id).filter((id): id is number | string => id != null)
  if (validIds.length > 0 && selected.size === validIds.length) {
    selected.clear()
  } else {
    validIds.forEach(id => selected.add(String(id)))
  }
}

const handleBatchFollow = () => {
  const ids = props.games
    .filter(g => g.id != null && selected.has(String(g.id)))
    .map(g => g.id!)
  if (ids.length === 0) return
  emit('batch-follow', ids)
  emit('update:visible', false)
  selected.clear()
}

const selectedCount = computed(() => selected.size)
const isIndeterminate = computed(() => selectedCount.value > 0 && selectedCount.value < props.games.length)
const isAllSelected = computed(() => props.games.length > 0 && selectedCount.value === props.games.length)

const formatHours = (minutes?: number | string) => {
  const m = Number(minutes ?? 0)
  return Math.round(m / 60)
}
</script>

<template>
  <Dialog :visible="props.visible" :closable="true" :breakpoints="{'960px': '75vw','640px': '90vw'}" @update:visible="emit('update:visible', $event)">
    <template #header>
      <span class="text-lg font-semibold">Select games to follow</span>
    </template>

    <div class="flex items-center justify-between mb-3">
      <Checkbox :checked="isAllSelected" :indeterminate="isIndeterminate" binary @click="toggleSelectAll" />
      <span class="text-sm text-muted-color">{{ selectedCount }} of {{ props.games.length }} selected</span>
    </div>

    <div class="max-h-[60vh] overflow-auto pr-2">
      <div v-for="game in props.games" :key="String(game.steamAppId)" class="flex items-center gap-2 p-2 border-b border-surface cursor-pointer" @click="game.id != null && toggleSelect(game.id)">
        <Checkbox 
          :model-value="game.id != null && selected.has(String(game.id))" 
          binary 
          @click.stop
          @update:model-value="val => game.id != null && toggleSelect(game.id)"
        />
        <div class="flex items-center gap-2 overflow-hidden">
          <img v-if="game.coverUrl" :src="game.coverUrl" class="w-8 h-12 rounded object-cover" />
          <span class="text-sm truncate max-w-[200px]">{{ game.title }}</span>
        </div>
        <span class="ml-auto text-xs text-muted-color">{{ formatHours(game.playtimeForeverMinutes) }}h</span>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-content-between align-items-center">
        <Button label="Cancel" outlined @click="emit('update:visible', false)" />
        <Button label="Follow selected" :disabled="selectedCount === 0 || props.isImporting" :loading="props.isImporting" @click="handleBatchFollow" />
      </div>
    </template>
  </Dialog>
</template>
