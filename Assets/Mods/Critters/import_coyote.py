import bpy
import sys
from os.path import dirname

# Import timbermesh plugin
sys.path.insert(0, '/tmp/timbermesh_plugin')
from timbermesh_blender_plugin import timbermesh_exporter
from timbermesh_blender_plugin import blender_utils

# Import FBX
print(f"Importing FBX from: /Users/pmcdavid/Downloads/low-poly-coyote-canis-latrans/source/Coyote.fbx")
bpy.ops.import_scene.fbx(filepath="/Users/pmcdavid/Downloads/low-poly-coyote-canis-latrans/source/Coyote.fbx")

# Create a collection for the coyote
collection = bpy.data.collections.new("Coyote")
bpy.context.scene.collection.children.link(collection)

# Move all imported objects to the collection
for obj in bpy.context.selected_objects:
    # Remove from all current collections
    for col in obj.users_collection:
        col.objects.unlink(obj)
    # Add to new collection
    collection.objects.link(obj)

print(f"Imported {len(collection.objects)} objects into collection '{collection.name}'")

# Print object names
for obj in collection.objects:
    print(f"  - {obj.name} ({obj.type})")

# Export to timbermesh
output_path = "/Users/pmcdavid/Projects/critters/Data/Entities/Coyote/Coyote.Model.timbermesh"
print(f"Exporting to: {output_path}")

# Create export settings
export_settings = timbermesh_exporter.ExportSettings(
    bpy.context,
    merge_meshes=True,
    single_animation=True,
    use_vertex_animations=False
)

# Export the collection
timbermesh_exporter.Exporter.export_collection(collection, output_path, export_settings)

print("Done!")
