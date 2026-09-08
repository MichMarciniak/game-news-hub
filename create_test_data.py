import json

# Take json from /Event/normalize

with open("normalized_names.json", "r", encoding="utf-8") as f:
    data = json.load(f)
    
for item in data:
    item["expected"] = item.pop("normalizedName")
    
with open("test_normalize.json", "w", encoding="utf-8") as f:
    json.dump(data, f, ensure_ascii=False, indent=2)