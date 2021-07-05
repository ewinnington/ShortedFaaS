import json
json_input = json.loads(input())

result = { "c" : float( json_input["a"]) * float( json_input["b"])}

print(json.dumps(result))