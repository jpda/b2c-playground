# oob-password-validation

This example shows how to perform password validation with `InputValidators` and out-of-band with a REST call (an Azure Function, in this case).

All changes for this to work are in [SignUpOrSignIn.xml](https://github.com/jpda/b2c-playground/blob/oob-password-validation/TF/LocalAccounts/SignUpOrSignin.xml). The other files are the originals from the starterpack, which only need to be updated to include your B2C tenant name in the header.

This includes:
- Password complexity policies
 - Length upper and lower bounds
 - At least one capital letter
 - At least one number
 - At least one symbol
- REST call to validate password doesn't contain username