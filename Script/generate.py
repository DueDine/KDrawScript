import os
import re
import json

FOLDERS = ['Dawntrail', 'Endwalker', 'Shadowbringers', 'Stormblood', 'Heavensward', 'ARealmReborn']
OUTPUT = 'Repo.json'
LINK = "https://raw.githubusercontent.com/DueDine/KDrawScript/main/"

pattern = re.compile(
    r'\[ScriptType\(name: "(.*?)", territorys: \[(.*?)\], guid: "(.*?)", version: "(.*?)", author: "(.*?)".*\)\]'
)

def parse(folder):
    entries = []
    for root, _, files in os.walk(folder):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                with open(file_path, "r", encoding="utf-8") as f:
                    content = f.read()
                    matches = pattern.findall(content)
                    for match in matches:
                        name, territorys, guid, version, author = match
                        if "Deprecated" in name:
                            continue
                        territory_ids = [int(t.strip()) for t in territorys.split(',') if t.strip().isdigit()]
                        entry = {
                            "Name": name,
                            "Guid": guid,
                            "Version": version,
                            "Author": author,
                            "TerritoryIds": territory_ids,
                            "DownloadUrl": f"{LINK}{os.path.relpath(file_path).replace(os.sep, '/')}"
                        }
                        entries.append(entry)
    return entries

def generate_json():
    data = []
    for folder in FOLDERS:
        entries = parse(folder)
        data.extend(entries)

    with open(OUTPUT, "w", encoding="utf-8") as f:
        json.dump(data, f, indent=4, ensure_ascii=False)
    
    print(f"Generated {OUTPUT}")

if __name__ == "__main__":
    generate_json()