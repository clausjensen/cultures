#Importing and exporting cultures

####Update: wrote a blog post on this issue http://claus.nu/blog/importing-custom-cultures-on-an-azure-vm/####

We had an issue where a site needed to use cultures that did not exist on our Azure VMs (Windows Server 2012 R2).

Since the cultures actually existed on our developer machines, we thought it would be possible to just export this culture and import it on the server.
There's however a few quirks you need to work around to make this possible.

- When exporting you may need to modify the `CultureAndRegionModifiers.None` flag depending on what kind of culture you are trying to transfer - I believe it can depend a bit whether the culture is a "specific supplemental culture" or one of the basic built-in ones (not entirely sure but you can tinker with this if it doesn't want to comply).

- Just importing the missing culture from the file will **not** work, as the static method `CultureAndRegionInfoBuilder.CreateFromLdml` apparently expects the culture name to already exist on the machine before allowing to load a culture from a file (...what?).

To work around this, I create a temporary culture using the name of the culture we want to import. Then I add some required data from an existing culture (doesn't matter which one as long as it exists - it's just temporary) and register the culture.

With this temporary culture registered, I am now able to load the *real* culture we want to import from the culture file. After doing that, I unregister the temporary culture and register the one loaded from the file instead.

There you go - the missing culture is now imported on the machine!

---

Honestly this seems like a pretty hacky workaround for something that seems like a bug - but it works :)
