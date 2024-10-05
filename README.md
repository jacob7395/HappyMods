This repo is for the mods I have developer for Quasimorph, they are currently being developed for the version
`beta - 0.8 beta`.
If you would like to contribute or use these mods as a base for your own follow the setup instructions to get started.

# Setup

## Pre requesits

### Netstandard 2.0

Download and install the latest [dotnet 2.0](https://dotnet.microsoft.com/en-us/download/dotnet/2.0), this is requiredd at present as Quasimorph has not updated to 2.1. When this happens 2.1 can be installed and used.

## Assemblies

Depending on where your steam is installed, you will need to add the remove, then attach the assemblies required.
These will be installed where evey you have Quasimorph installed,
for me this is `C:\Program Files (x86)\Steam\steamapps\common\Quasimorph\Quasimorph_Data\Managed`.
Note down the assemblies current attached and add the same assemblies from your install.
If steam is installed in the same path as mine this might not be required.

![alt text](/docs/attachments/assemblies.png)

## Attached folders

There are two folders that will be useful to attached to the solution.
Once attached they will be added to a file called `indexLayout.xaml`,
do not commit this to the repo as it's unique to each dev.
If you are using rider, you can create a `ChangeList` (I call it `Setup`) and leave it in there.
I may update this to be in the .gitignore in future or try and use an environment variable in the path for the steam
folder.

### Steam Workshop Output

Go to your steam install directory and locate the steam workshop folder for the subscribed mod.
This should be the same directory as you set for the `deployDir` in the project build events;

```
set deployDir="C:\Program Files (x86)\Steam\steamapps\workshop\content\2059170\3342973875\"
```

### Quasimorph Save Directory

Your Quasimorph for me this is located at `C:\Users\<USER>\AppData\LocalLow\Magnum Scriptum Ltd`.

## Checks

Once the above is done, delete all the content of in the steam workshop mod folder then run the solution build.
The folder should be repopulated by your files.