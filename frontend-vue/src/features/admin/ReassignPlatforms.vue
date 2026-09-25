<script setup lang="ts">
import { getGetPlatformGroupsFullQueryKey, useGetPlatformGroups, useGetPlatformGroupsFull, usePatchAdminPlatformReassign } from '@/api/generated';
import { useQueryClient } from '@tanstack/vue-query';
import { useToast } from 'openvue';
import { ref } from 'vue';

const toast = useToast()

const selectGroupId = ref<string | number | null>(null)
const selectPlatformId = ref<string | number | null>(null)

const queryClient = useQueryClient()

const {data: platformGroupsFull} = useGetPlatformGroupsFull()

const {data: platformGroups} = useGetPlatformGroups()
const {mutate: reassignPlatform} = usePatchAdminPlatformReassign()

function onSelect(id: number | string) {
  selectGroupId.value = id
}

function onSelectPlatform(id: number | string) {
  selectPlatformId.value = id
}

function onReassign() {
  if (!selectGroupId.value || !selectPlatformId.value) return

  reassignPlatform({params: 
    {
      groupId: selectGroupId.value, 
      platformId: selectPlatformId.value
    }
  }, {
    onSuccess: () => {
      queryClient.invalidateQueries({ 
        queryKey: getGetPlatformGroupsFullQueryKey() 
      })

      selectGroupId.value = null
      selectPlatformId.value = null
    },
    onError: () => {
      toast.add({severity: 'error', summary: 'Error reassigning platform', life: 1000})
    }
  })

}


</script>

<template>
  <div class="w-full -m-3 flex flex-col items-center relative">

    <Splitter class="flex h-full shrink-0 w-full max-h-[calc(100vh-5rem)]">
      <SplitterPanel class="overflow-hidden" :size="75" :min-size="20">
        <Accordion :value="[]" multiple class="overflow-y-scroll  h-full">
          <AccordionPanel v-for="group in platformGroupsFull" :key="group.id" :value="group.id">
            <AccordionHeader>{{ group.name }}</AccordionHeader>
            <AccordionContent>
              <ul class="flex flex-col gap-1">
                <li
                  v-for="p in group.platforms" :key="p.id"  
                  @click="onSelectPlatform(p.id!)"
                  class="flex items-center p-2 rounded-md cursor-pointer transition-colors text-sm"
                  :class="[selectPlatformId === p.id ? 'bg-primary text-primary-contrast font-medium' : 'hover:bg-surface-hover']"
                >
                  <span>{{ p.name }}</span>
                </li>
              </ul>
            </AccordionContent>
          </AccordionPanel>
          
        </Accordion>
      </SplitterPanel>
      <SplitterPanel class="flex flex-col gap-2 p-2" :size="25" :min-size="15">
        <Button
          v-for="g in platformGroups"
          :key="g.id"
          :label="g.name"
          fluid
          :variant="selectGroupId === g.id ? 'outlined' : 'solid'"
          @click="() => onSelect(g.id)"
          
        ></Button>
      </SplitterPanel>
    </Splitter>
    <div class="z-10 flex justify-center p-2 border-surface bg-content absolute -bottom-8">
      <Button 
        @click="onReassign" 
        label="Reassign" 
        :disabled="!selectGroupId || !selectPlatformId" 
      />
    </div>
  </div>
</template>