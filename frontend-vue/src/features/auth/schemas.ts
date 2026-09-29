import { zodResolver } from "@openvue/forms/resolvers/zod";
import z from "zod";

export const emailSchema = z
  .email({message: 'Invalid email'})
  .min(1, {message: 'Email is required.'})

export const usernameSchema = z
  .string()
  .min(3, {message: 'Mininum 3 characters.'})

export const passwordSchema = z
  .string()
  .min(8, {message: 'Minimum 8 characters'})

export const passwordNoCheck = z.
  string()
  .min(1, {message: 'Password is required'})

export const loginSchema = z.object({
  email: emailSchema,
  password: passwordNoCheck
})

export const registerSchema = z.object({
  username: usernameSchema,
  email: emailSchema,
  password: passwordSchema
})

export const resetPasswordRequestSchema = z.object({
  email: emailSchema
})

export const resetPasswordSchema = z.object({
  password: passwordSchema
})

export const changePasswordSchema = z.object({
  oldPassword: passwordNoCheck,
  newPassword: passwordSchema
})

export const loginResolver = zodResolver(loginSchema)
export const registerResolver = zodResolver(registerSchema)
export const resetPasswordRequestResolver = zodResolver(resetPasswordRequestSchema)
export const resetPasswordResolver = zodResolver(resetPasswordSchema)
export const changePasswordResolver = zodResolver(changePasswordSchema)