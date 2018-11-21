# password-reuse

Sometimes we're required for one reason or another to prevent password reuse. To do this, we need to store a history of the hashes of previous passwords.
## warning
We're dealing here with *cleartext passwords* leaving B2C and coming into your code, where they'll be hashed and stored in a storage provider of some sort. There are multiple considerations:
- Never ever store actual passwords as cleartext. Some parts of the code here do that for debug, but they are marked as such - never ever store a cleartext anything
- Don't roll your own crypto algorithms for password hashing, it's just a terrible idea
- This service could leak sensitive data, so it should be secured appropriately
- Just because a user's previous passwords can't authenticate them anymore doesn't mean we're not still putting them at risk - *treat your hash database as you would a current password database!*

*Last reminder - you should strongly consider alternatives before interacting with user's passwords.*

## What's new
For now, this uses Table Storage - I will update this in the near future to store the history in the Azure AD Graph, to at least get away from having to manage the store myself.
In here you'll find two new Azure Functions:
- [password-history-save.csx](https://github.com/jpda/b2c-playground/blob/password-history/func/password-history-save.csx): responsible for hashing and saving the passwords, in this case to Azure Table Storage.
- [password-history-check.csx](https://github.com/jpda/b2c-playground/blob/password-history/func/password-history-check.csx): responsible for checking a user's new desired password against the existing saved hashes.
- TrustFramework policies aren't updated yet to use these, that's coming soon.