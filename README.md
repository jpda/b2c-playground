# b2c-playground

In this repo, you'll find a few samples of specific tasks in B2C.

## Use the branches to see various tasks
- oob-password-validation

This example shows how to perform password validation with `InputValidators` and out-of-band with a REST call (an Azure Function, in this case).
All changes for this to work are in [SignUpOrSignIn.xml](https://github.com/jpda/b2c-playground/blob/oob-password-validation/TF/LocalAccounts/SignUpOrSignin.xml). This includes:
- Password complexity policies
 - Length upper and lower bounds
 - At least one capital letter
 - At least one number
 - At least one symbol
- REST call to validate password doesn't contain username